using System.Text.Json;
using TravelApi.DTOs;

namespace TravelApi.Services
{
    
    public interface IWeatherService
    {
        Task<WeatherInfoDto?> GetWeatherAsync(string city);
    }

    
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<WeatherService> _logger;

        public WeatherService(HttpClient httpClient, IConfiguration configuration, ILogger<WeatherService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;

            
            _httpClient.BaseAddress = new Uri("https://api.openweathermap.org/data/2.5/");
        }

        public async Task<WeatherInfoDto?> GetWeatherAsync(string city)
        {
            try
            {
                
                var apiKey = _configuration["WeatherApiKey"];

                
                var response = await _httpClient.GetAsync($"weather?q={city}&appid={apiKey}&units=metric");

                
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                
                using var jsonDocument = JsonDocument.Parse(content);
                var temp = jsonDocument.RootElement.GetProperty("main").GetProperty("temp").GetDouble();
                var desc = jsonDocument.RootElement.GetProperty("weather")[0].GetProperty("description").GetString();

                return new WeatherInfoDto
                {
                    City = city,
                    Temperature = temp,
                    Description = desc ?? "No description"
                };
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error fetching weather data for {City}", city);
                return null;
            }
        }
    }
}