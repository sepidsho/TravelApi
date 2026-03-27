namespace TravelApi.DTOs
{
    public class TourResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int DurationDays { get; set; }
        public string DestinationName { get; set; } = string.Empty;
        public WeatherInfoDto? Weather { get; set; }
    }
}