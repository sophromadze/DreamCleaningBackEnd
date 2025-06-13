using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using DreamCleaningBackend.DTOs;
using DreamCleaningBackend.Services.Interfaces;

namespace DreamCleaningBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet]
        public async Task<ActionResult<ProfileDto>> GetProfile()
        {
            try
            {
                var userId = GetUserId();
                var profile = await _profileService.GetProfile(userId);
                return Ok(profile);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<ActionResult<ProfileDto>> UpdateProfile(UpdateProfileDto updateProfileDto)
        {
            try
            {
                var userId = GetUserId();
                var profile = await _profileService.UpdateProfile(userId, updateProfileDto);
                return Ok(profile);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("apartments")]
        public async Task<ActionResult<List<ApartmentDto>>> GetApartments()
        {
            try
            {
                var userId = GetUserId();
                var apartments = await _profileService.GetUserApartments(userId);
                return Ok(apartments);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("apartments")]
        public async Task<ActionResult<ApartmentDto>> AddApartment(CreateApartmentDto createApartmentDto)
        {
            try
            {
                var userId = GetUserId();
                var apartment = await _profileService.AddApartment(userId, createApartmentDto);
                return Ok(apartment);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("apartments/{apartmentId}")]
        public async Task<ActionResult<ApartmentDto>> UpdateApartment(int apartmentId, ApartmentDto apartmentDto)
        {
            try
            {
                var userId = GetUserId();
                var apartment = await _profileService.UpdateApartment(userId, apartmentId, apartmentDto);
                return Ok(apartment);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("apartments/{apartmentId}")]
        public async Task<ActionResult> DeleteApartment(int apartmentId)
        {
            try
            {
                var userId = GetUserId();
                await _profileService.DeleteApartment(userId, apartmentId);
                return Ok(new { message = "Apartment deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private int GetUserId()
        {
            // Try "UserId" first (what your JWT probably uses), then fallback to NameIdentifier
            var userIdClaim = User.FindFirst("UserId")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                throw new Exception("Invalid user");

            return userId;
        }
    }
}