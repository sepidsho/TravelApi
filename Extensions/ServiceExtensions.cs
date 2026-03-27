using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace TravelApi.Extensions
{
    public static class ServiceExtensions
    {
        
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("SpecificOriginPolicy", builder =>
                    builder.WithOrigins("http://localhost:5173")  
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                         
                           .WithExposedHeaders("X-Pagination"));
            });
        }

        public static void ConfigureRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.AddFixedWindowLimiter("FixedWindowPolicy", opt =>
                {
                    opt.Window = TimeSpan.FromSeconds(10);
                    opt.PermitLimit = 5; 
                    opt.QueueLimit = 2; 
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                });
            });
        }
    }
}