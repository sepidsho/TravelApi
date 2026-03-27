namespace TravelApi.Models
{
    public class Tour
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int DurationDays { get; set; }

        
        public int DestinationId { get; set; }
        public Destination? Destination { get; set; }
    }
}