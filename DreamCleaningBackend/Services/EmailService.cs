using DreamCleaningBackend.Services.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;


namespace DreamCleaningBackend.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailVerificationAsync(string email, string firstName, string verificationLink)
        {
            var subject = "Verify Your Email - Dream Cleaning";
            var body = $@"
                <h2>Hi {firstName},</h2>
                <p>Thank you for registering with Dream Cleaning!</p>
                <p>Please click the button below to verify your email address:</p>
                <p style='margin: 30px 0;'>
                    <a href='{verificationLink}' 
                       style='background-color: #4CAF50; color: white; padding: 14px 20px; 
                              text-decoration: none; border-radius: 4px; display: inline-block;'>
                        Verify Email
                    </a>
                </p>
                <p>Or copy and paste this link into your browser:</p>
                <p>{verificationLink}</p>
                <p>This link will expire in 24 hours.</p>
                <p>If you didn't create an account, please ignore this email.</p>
                <br/>
                <p>Best regards,<br/>Dream Cleaning Team</p>
            ";

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendPasswordResetAsync(string email, string firstName, string resetLink)
        {
            var subject = "Password Reset Request - Dream Cleaning";
            var body = $@"
                <h2>Hi {firstName},</h2>
                <p>We received a request to reset your password.</p>
                <p>Click the button below to reset your password:</p>
                <p style='margin: 30px 0;'>
                    <a href='{resetLink}' 
                       style='background-color: #2196F3; color: white; padding: 14px 20px; 
                              text-decoration: none; border-radius: 4px; display: inline-block;'>
                        Reset Password
                    </a>
                </p>
                <p>Or copy and paste this link into your browser:</p>
                <p>{resetLink}</p>
                <p>This link will expire in 1 hour.</p>
                <p>If you didn't request a password reset, please ignore this email.</p>
                <br/>
                <p>Best regards,<br/>Dream Cleaning Team</p>
            ";

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendWelcomeEmailAsync(string email, string firstName)
        {
            var subject = "Welcome to Dream Cleaning!";
            var body = $@"
                <h2>Welcome {firstName}!</h2>
                <p>Your email has been verified successfully.</p>
                <p>You can now enjoy all the features of Dream Cleaning:</p>
                <ul>
                    <li>Book cleaning services</li>
                    <li>Manage your apartments</li>
                    <li>Track your orders</li>
                    <li>Subscribe for discounts</li>
                </ul>
                <p>If you have any questions, feel free to contact our support team.</p>
                <br/>
                <p>Best regards,<br/>Dream Cleaning Team</p>
            ";

            await SendEmailAsync(email, subject, body);
        }

        private async Task SendEmailAsync(string to, string subject, string html)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(
                    _configuration["Email:FromName"],
                    _configuration["Email:FromAddress"]
                ));
                email.To.Add(MailboxAddress.Parse(to));
                email.Subject = subject;
                email.Body = new TextPart(TextFormat.Html) { Text = html };

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(
                    _configuration["Email:SmtpHost"],
                    int.Parse(_configuration["Email:SmtpPort"]),
                    SecureSocketOptions.StartTls
                );
                await smtp.AuthenticateAsync(
                    _configuration["Email:SmtpUser"],
                    _configuration["Email:SmtpPassword"]
                );
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                _logger.LogInformation($"Email sent successfully to {to}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {to}");
                throw;
            }
        }
    }
}