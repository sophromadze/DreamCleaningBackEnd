namespace DreamCleaningBackend.DTOs
{
    public class FrequencyDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal DiscountPercentage { get; set; }
        public int FrequencyDays { get; set; }
    }
}
