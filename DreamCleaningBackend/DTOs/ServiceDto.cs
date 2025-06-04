namespace DreamCleaningBackend.DTOs
{
    public class ServiceDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ServiceKey { get; set; }
        public decimal Cost { get; set; }
        public int TimeDuration { get; set; }
        public int ServiceTypeId { get; set; }
        public string InputType { get; set; }
        public int? MinValue { get; set; }
        public int? MaxValue { get; set; }
        public int? StepValue { get; set; }
        public bool IsRangeInput { get; set; }
        public string? Unit { get; set; }
    }
}
