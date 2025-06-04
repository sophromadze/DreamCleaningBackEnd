namespace DreamCleaningBackend.DTOs
{
    public class ExtraServiceDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; }
        public string? Icon { get; set; }
        public bool HasQuantity { get; set; }
        public bool HasHours { get; set; }
        public bool IsDeepCleaning { get; set; }
        public bool IsSuperDeepCleaning { get; set; }
        public bool IsSameDayService { get; set; }
        public decimal PriceMultiplier { get; set; }
        public bool IsAvailableForAll { get; set; }
    }
}
