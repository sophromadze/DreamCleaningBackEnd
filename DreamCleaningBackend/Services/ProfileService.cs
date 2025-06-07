using DreamCleaningBackend.DTOs;
using DreamCleaningBackend.Models;
using DreamCleaningBackend.Repositories.Interfaces;
using DreamCleaningBackend.Services.Interfaces;

namespace DreamCleaningBackend.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IUserRepository _userRepository;
        private readonly IApartmentRepository _apartmentRepository;

        public ProfileService(IUserRepository userRepository, IApartmentRepository apartmentRepository)
        {
            _userRepository = userRepository;
            _apartmentRepository = apartmentRepository;
        }

        public async Task<ProfileDto> GetProfile(int userId)
        {
            var user = await _userRepository.GetByIdWithDetailsAsync(userId);

            if (user == null)
                throw new Exception("User not found");

            return MapUserToProfileDto(user);
        }

        public async Task<ProfileDto> UpdateProfile(int userId, UpdateProfileDto updateProfileDto)
        {
            var user = await _userRepository.GetByIdWithDetailsAsync(userId);

            if (user == null)
                throw new Exception("User not found");

            // Check if email is being changed and if it's already taken
            if (user.Email.ToLower() != updateProfileDto.Email.ToLower())
            {
                if (await _userRepository.UserExistsAsync(updateProfileDto.Email))
                    throw new Exception("Email address is already in use");
            }

            user.FirstName = updateProfileDto.FirstName;
            user.LastName = updateProfileDto.LastName;
            user.Email = updateProfileDto.Email.ToLower();
            user.Phone = updateProfileDto.Phone;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            return MapUserToProfileDto(user);
        }

        public async Task<ApartmentDto> AddApartment(int userId, CreateApartmentDto createApartmentDto)
        {
            // Get user with apartments to check count
            var user = await _userRepository.GetByIdWithDetailsAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            // Check if user has reached the maximum number of apartments
            if (user.Apartments.Count >= 5)
                throw new Exception("You have reached the maximum limit of 5 apartments");

            var apartment = new Apartment
            {
                Name = createApartmentDto.Name,
                Address = createApartmentDto.Address,
                AptSuite = createApartmentDto.AptSuite,
                City = createApartmentDto.City,
                State = createApartmentDto.State,
                PostalCode = createApartmentDto.PostalCode,
                SpecialInstructions = createApartmentDto.SpecialInstructions,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _apartmentRepository.CreateAsync(apartment);
            await _apartmentRepository.SaveChangesAsync();

            return MapApartmentToDto(apartment);
        }

        public async Task<ApartmentDto> UpdateApartment(int userId, int apartmentId, ApartmentDto apartmentDto)
        {
            // Verify apartment belongs to user
            if (!await _apartmentRepository.BelongsToUserAsync(apartmentId, userId))
                throw new Exception("Apartment not found");

            var apartment = await _apartmentRepository.GetByIdAsync(apartmentId);
            if (apartment == null)
                throw new Exception("Apartment not found");

            apartment.Name = apartmentDto.Name;
            apartment.Address = apartmentDto.Address;
            apartment.AptSuite = apartmentDto.AptSuite;
            apartment.City = apartmentDto.City;
            apartment.State = apartmentDto.State;
            apartment.PostalCode = apartmentDto.PostalCode;
            apartment.SpecialInstructions = apartmentDto.SpecialInstructions;
            apartment.UpdatedAt = DateTime.UtcNow;

            await _apartmentRepository.UpdateAsync(apartment);
            await _apartmentRepository.SaveChangesAsync();

            return MapApartmentToDto(apartment);
        }

        public async Task<bool> DeleteApartment(int userId, int apartmentId)
        {
            // Verify apartment belongs to user
            if (!await _apartmentRepository.BelongsToUserAsync(apartmentId, userId))
                throw new Exception("Apartment not found");

            var result = await _apartmentRepository.DeleteAsync(apartmentId);
            if (!result)
                throw new Exception("Failed to delete apartment");

            await _apartmentRepository.SaveChangesAsync();
            return true;
        }

        public async Task<List<ApartmentDto>> GetUserApartments(int userId)
        {
            var apartments = await _apartmentRepository.GetByUserIdAsync(userId);
            return apartments.Select(MapApartmentToDto).ToList();
        }

        private ProfileDto MapUserToProfileDto(User user)
        {
            return new ProfileDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                FirstTimeOrder = user.FirstTimeOrder,
                SubscriptionId = user.SubscriptionId,
                SubscriptionName = user.Subscription?.Name,
                Apartments = user.Apartments.Select(MapApartmentToDto).ToList()
            };
        }

        private ApartmentDto MapApartmentToDto(Apartment apartment)
        {
            return new ApartmentDto
            {
                Id = apartment.Id,
                Name = apartment.Name,
                Address = apartment.Address,
                AptSuite = apartment.AptSuite,
                City = apartment.City,
                State = apartment.State,
                PostalCode = apartment.PostalCode,
                SpecialInstructions = apartment.SpecialInstructions
            };
        }
    }
}