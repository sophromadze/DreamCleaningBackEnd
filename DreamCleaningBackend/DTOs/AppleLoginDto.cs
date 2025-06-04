using System.ComponentModel.DataAnnotations;

namespace DreamCleaningBackend.DTOs
{
    public class AppleLoginDto
    {
        [Required]
        public string IdToken { get; set; }

        // Apple sometimes doesn't return email in subsequent logins
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
