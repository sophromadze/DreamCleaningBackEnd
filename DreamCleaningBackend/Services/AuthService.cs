using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DreamCleaningBackend.Data;
using DreamCleaningBackend.DTOs;
using DreamCleaningBackend.Models;
using Google.Apis.Auth;
using DreamCleaningBackend.Services.Interfaces;

namespace DreamCleaningBackend.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.Email))
                throw new Exception("User already exists");

            CreatePasswordHash(registerDto.Password, out string passwordHash, out string passwordSalt);

            var user = new User
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email.ToLower(),
                Phone = registerDto.Phone,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                AuthProvider = "Local",
                CreatedAt = DateTime.UtcNow,
                FirstTimeOrder = true
            };

            // Generate refresh token
            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                User = MapUserToDto(user),
                Token = CreateToken(user),
                RefreshToken = user.RefreshToken
            };
        }

        public async Task<AuthResponseDto> Login(LoginDto loginDto)
        {
            var user = await _context.Users
                .Include(u => u.Subscription)
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email.ToLower());

            if (user == null || user.AuthProvider != "Local")
                throw new Exception("Invalid email or password");

            if (!user.IsActive)
                throw new Exception("Account is deactivated");

            if (!VerifyPasswordHash(loginDto.Password, user.PasswordHash, user.PasswordSalt))
                throw new Exception("Invalid email or password");

            // Update refresh token
            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                User = MapUserToDto(user),
                Token = CreateToken(user),
                RefreshToken = user.RefreshToken
            };
        }

        public async Task<AuthResponseDto> GoogleLogin(GoogleLoginDto googleLoginDto)
        {
            try
            {
                // Validate Google token
                var payload = await ValidateGoogleToken(googleLoginDto.IdToken);

                if (payload == null)
                    throw new Exception("Invalid Google token");

                var email = payload.Email;
                var googleId = payload.Subject;
                var firstName = payload.GivenName ?? "User";
                var lastName = payload.FamilyName ?? "";

                // Check if user exists
                var user = await _context.Users
                    .Include(u => u.Subscription)
                    .FirstOrDefaultAsync(u => u.Email == email.ToLower());

                if (user == null)
                {
                    // Create new user
                    user = new User
                    {
                        Email = email.ToLower(),
                        FirstName = firstName,
                        LastName = lastName,
                        AuthProvider = "Google",
                        ExternalAuthId = googleId,
                        CreatedAt = DateTime.UtcNow,
                        FirstTimeOrder = true,
                        IsActive = true
                    };

                    _context.Users.Add(user);
                }
                else if (user.AuthProvider != "Google")
                {
                    throw new Exception("Email already registered with different provider");
                }

                // Update refresh token
                user.RefreshToken = GenerateRefreshToken();
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                await _context.SaveChangesAsync();

                return new AuthResponseDto
                {
                    User = MapUserToDto(user),
                    Token = CreateToken(user),
                    RefreshToken = user.RefreshToken
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Google login failed: {ex.Message}");
            }
        }

        public async Task<AuthResponseDto> AppleLogin(AppleLoginDto appleLoginDto)
        {
            // Apple login implementation would go here
            // For now, throwing not implemented
            throw new NotImplementedException("Apple login not yet implemented");

            // In a real implementation, you would:
            // 1. Validate Apple ID token
            // 2. Extract user info
            // 3. Create or update user
            // 4. Return auth response
        }

        public async Task<AuthResponseDto> RefreshToken(RefreshTokenDto refreshTokenDto)
        {
            var principal = GetPrincipalFromExpiredToken(refreshTokenDto.Token);
            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                throw new Exception("Invalid token");

            var user = await _context.Users
                .Include(u => u.Subscription)
                .FirstOrDefaultAsync(u => u.Id == int.Parse(userId));

            if (user == null ||
                user.RefreshToken != refreshTokenDto.RefreshToken ||
                user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                throw new Exception("Invalid refresh token");
            }

            // Generate new tokens
            var newAccessToken = CreateToken(user);
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                User = MapUserToDto(user),
                Token = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }

        public async Task<AuthResponseDto> RefreshUserToken(int userId)
        {
            // Get fresh user data from database
            var user = await _context.Users
                .Include(u => u.Subscription)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new Exception("User not found");

            if (!user.IsActive)
                throw new Exception("User account is blocked");

            // Generate new token with fresh role
            var token = CreateToken(user);
            var refreshToken = GenerateRefreshToken();

            // Update refresh token in database
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                Token = token,
                RefreshToken = refreshToken,
                User = new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Phone = user.Phone,
                    Role = user.Role.ToString(),
                    FirstTimeOrder = user.FirstTimeOrder
                }
            };
        }


        public async Task<bool> UserExists(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email.ToLower());
        }

        private async Task<GoogleJsonWebSignature.Payload> ValidateGoogleToken(string idToken)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string>() { _configuration["Authentication:Google:ClientId"] }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
                return payload;
            }
            catch
            {
                return null;
            }
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AppSettings:Token"])),
                ValidateLifetime = false // Don't validate lifetime for expired tokens
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        private void CreatePasswordHash(string password, out string passwordHash, out string passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                var salt = hmac.Key;
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                passwordSalt = Convert.ToBase64String(salt);
                passwordHash = Convert.ToBase64String(hash);
            }
        }

        private bool VerifyPasswordHash(string password, string storedHash, string storedSalt)
        {
            var salt = Convert.FromBase64String(storedSalt);
            using (var hmac = new HMACSHA512(salt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                var hash = Convert.FromBase64String(storedHash);
                return computedHash.SequenceEqual(hash);
            }
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
        // IMPORTANT: Add both ClaimTypes.Role and custom "Role" claim
        new Claim(ClaimTypes.Role, user.Role.ToString()),
        new Claim("Role", user.Role.ToString()), 
        new Claim("UserId", user.Id.ToString()), 
        new Claim("FirstName", user.FirstName),
        new Claim("LastName", user.LastName),
        new Claim("FirstTimeOrder", user.FirstTimeOrder.ToString()),
        new Claim("AuthProvider", user.AuthProvider ?? "Local")
    };

            if (user.SubscriptionId.HasValue)
            {
                claims.Add(new Claim("SubscriptionId", user.SubscriptionId.Value.ToString()));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1), // Short-lived access token
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public async Task<bool> ChangePassword(int userId, ChangePasswordDto changePasswordDto)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                throw new Exception("User not found");

            // Only allow password change for local accounts
            if (user.AuthProvider != "Local")
                throw new Exception("Password change is not allowed for OAuth accounts");

            // Verify current password
            if (!VerifyPasswordHash(changePasswordDto.CurrentPassword, user.PasswordHash, user.PasswordSalt))
                throw new Exception("Current password is incorrect");

            // Create new password hash
            CreatePasswordHash(changePasswordDto.NewPassword, out string passwordHash, out string passwordSalt);

            // Update user password
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }

        private UserDto MapUserToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                FirstTimeOrder = user.FirstTimeOrder,
                SubscriptionId = user.SubscriptionId,
                AuthProvider = user.AuthProvider,
                Role = user.Role.ToString()
            };
        }
    }
}