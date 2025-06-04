﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DreamCleaningBackend.Models
{
    public class ServiceType
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } // e.g., "Residential Cleaning", "Office Cleaning"

        [Column(TypeName = "decimal(18,2)")]
        public decimal BasePrice { get; set; } // Base price for this service type

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        // Display order for UI
        public int DisplayOrder { get; set; }

        // Navigation properties
        public virtual ICollection<Service> Services { get; set; } = new List<Service>();
        public virtual ICollection<ExtraService> ExtraServices { get; set; } = new List<ExtraService>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

        // Audit fields
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}