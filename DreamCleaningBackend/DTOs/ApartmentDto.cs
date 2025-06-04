using System.ComponentModel.DataAnnotations;

namespace DreamCleaningBackend.DTOs
{
    public class ApartmentDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

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

        [StringLength(500)]
        public string? SpecialInstructions { get; set; }
    }
}
