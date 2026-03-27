using Polly;
using Polly.Extensions.Http;
using TravelApi.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using TravelApi.Data;
using TravelApi.Extensions;

namespace TravelApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddMemoryCache();


            builder.Services.ConfigureCors();
            builder.Services.ConfigureRateLimiting();

            builder.Services.AddDbContext<TravelDbContext>(options =>
                options.UseInMemoryDatabase("TravelDb"));

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHttpClient<IWeatherService, WeatherService>()

           .AddPolicyHandler(HttpPolicyExtensions
           .HandleTransientHttpError() 
           .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            
            app.UseCors("SpecificOriginPolicy");
            app.UseRateLimiter();

            app.UseAuthorization();

            app.MapControllers().RequireRateLimiting("FixedWindowPolicy");

            app.Run();
        }
    }
}