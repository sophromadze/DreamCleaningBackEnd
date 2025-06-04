using System.ComponentModel.DataAnnotations;

namespace DreamCleaningBackend.DTOs
{
    // Service DTOs
    public class CreateServiceDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string ServiceKey { get; set; }
        [Required]
        public decimal Cost { get; set; }
        [Required]
        public int TimeDuration { get; set; }
        [Required]
        public int ServiceTypeId { get; set; }
        [Required]
        public string InputType { get; set; } = "dropdown";
        public int? MinValue { get; set; }
        public int? MaxValue { get; set; }
        public int? StepValue { get; set; }
        public bool IsRangeInput { get; set; } = false;
        public string? Unit { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class UpdateServiceDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string ServiceKey { get; set; }
        [Required]
        public decimal Cost { get; set; }
        [Required]
        public int TimeDuration { get; set; }
        [Required]
        public int ServiceTypeId { get; set; }
        [Required]
        public string InputType { get; set; }
        public int? MinValue { get; set; }
        public int? MaxValue { get; set; }
        public int? StepValue { get; set; }
        public bool IsRangeInput { get; set; }
        public string? Unit { get; set; }
        public int DisplayOrder { get; set; }
    }

    // Extra Service DTOs
    public class CreateExtraServiceDto
    {
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int Duration { get; set; }
        public string? Icon { get; set; }
        public bool HasQuantity { get; set; }
        public bool HasHours { get; set; }
        public bool IsDeepCleaning { get; set; }
        public bool IsSuperDeepCleaning { get; set; }
        public bool IsSameDayService { get; set; }
        public decimal PriceMultiplier { get; set; } = 1.0m;
        public int? ServiceTypeId { get; set; }
        public bool IsAvailableForAll { get; set; } = true;
        public int DisplayOrder { get; set; }
    }

    public class UpdateExtraServiceDto
    {
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int Duration { get; set; }
        public string? Icon { get; set; }
        public bool HasQuantity { get; set; }
        public bool HasHours { get; set; }
        public bool IsDeepCleaning { get; set; }
        public bool IsSuperDeepCleaning { get; set; }
        public bool IsSameDayService { get; set; }
        public decimal PriceMultiplier { get; set; }
        public int? ServiceTypeId { get; set; }
        public bool IsAvailableForAll { get; set; }
        public int DisplayOrder { get; set; }
    }

    // Frequency DTOs
    public class CreateFrequencyDto
    {
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        [Required]
        public decimal DiscountPercentage { get; set; }
        [Required]
        public int FrequencyDays { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class UpdateFrequencyDto
    {
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        [Required]
        public decimal DiscountPercentage { get; set; }
        [Required]
        public int FrequencyDays { get; set; }
        public int DisplayOrder { get; set; }
    }
}