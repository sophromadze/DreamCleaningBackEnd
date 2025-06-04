namespace DreamCleaningBackend.DTOs
{
    public class ServiceTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal BasePrice { get; set; }
        public string? Description { get; set; }
        public List<ServiceDto> Services { get; set; } = new List<ServiceDto>();
        public List<ExtraServiceDto> ExtraServices { get; set; } = new List<ExtraServiceDto>();
    }
}
