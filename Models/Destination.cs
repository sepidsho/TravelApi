namespace TravelApi.Models
{
    public class Destination
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public ICollection<Tour> Tours { get; set; } = new List<Tour>();
    }
}