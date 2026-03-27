namespace TravelApi.DTOs
{
    public class WeatherInfoDto
    {
        public string City { get; set; } = string.Empty;
        public double Temperature { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}