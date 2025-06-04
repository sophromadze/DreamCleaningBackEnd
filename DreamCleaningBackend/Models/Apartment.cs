using System.ComponentModel.DataAnnotations;

namespace DreamCleaningBackend.Models
{
    public class Apartment
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } // e.g., "My Home", "Beach House"

        [Required]
        [StringLength(200)]
        public string Address { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(20)]
        public string? PostalCode { get; set; }

        public int? NumberOfRooms { get; set; }
        public int? NumberOfBathrooms { get; set; }
        public double? SquareMeters { get; set; }

        // Special instructions for cleaners
        [StringLength(500)]
        public string? SpecialInstructions { get; set; }

        // Foreign key
        public int UserId { get; set; }
        public virtual User User { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
