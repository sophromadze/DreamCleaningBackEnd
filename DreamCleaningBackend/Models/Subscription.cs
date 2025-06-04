using System.ComponentModel.DataAnnotations;

namespace DreamCleaningBackend.Models
{
    public class Subscription
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } // e.g., "Weekly", "Bi-Weekly", "Monthly"

        [StringLength(200)]
        public string? Description { get; set; }

        // Frequency in days
        public int FrequencyDays { get; set; } // 7 for weekly, 14 for bi-weekly, etc.

        // Discount percentage for subscription
        public decimal DiscountPercentage { get; set; } = 0;

        // Price modifier (if any)
        public decimal PriceModifier { get; set; } = 1.0m;

        public bool IsActive { get; set; } = true;

        // Navigation property
        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}
