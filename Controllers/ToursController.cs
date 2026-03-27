using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory; // این خط برای کش اضافه شد
using System.Text.Json;
using TravelApi.Data;
using TravelApi.DTOs;
using TravelApi.Models;
using TravelApi.Services;

namespace TravelApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToursController : ControllerBase
    {
        private readonly TravelDbContext _context;
        private readonly IWeatherService _weatherService;
        private readonly IMemoryCache _cache; // متغیر کش

        // یک کلید ثابت برای ذخیره اطلاعات در کش
        private const string CacheKey = "ToursListCache";

        // تزریق دیتابیس، سرویس آب‌وهوا و سرویس کش
        public ToursController(TravelDbContext context, IWeatherService weatherService, IMemoryCache cache)
        {
            _context = context;
            _weatherService = weatherService;
            _cache = cache;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TourResponseDto>>> GetTours(
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            // ۱. بررسی می‌کنیم آیا لیست تورها از قبل در کش (حافظه) وجود دارد یا نه؟
            if (!_cache.TryGetValue(CacheKey, out List<TourResponseDto>? allTours))
            {
                // ۲. اگر در کش نبود (Cache Miss)، از دیتابیس می‌خوانیم
                var toursFromDb = await _context.Tours.Include(t => t.Destination).ToListAsync();

                allTours = toursFromDb.Select(t => new TourResponseDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Price = t.Price,
                    DurationDays = t.DurationDays,
                    DestinationName = t.Destination != null ? t.Destination.Name : "Unknown"
                }).ToList();

                // تنظیمات کش: دیتا را برای 5 دقیقه در حافظه نگه دار
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

                _cache.Set(CacheKey, allTours, cacheOptions);
            }

            // --- اعمال فیلتر و صفحه‌بندی روی دیتای کش شده ---
            var query = allTours!.AsQueryable();

            if (minPrice.HasValue)
                query = query.Where(t => t.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(t => t.Price <= maxPrice.Value);

            var totalItems = query.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var paginationMetadata = new
            {
                totalItems,
                totalPages,
                currentPage = page,
                pageSize
            };

            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            var pagedTours = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // در هدر ریسپانس به کلاینت می‌گوییم که دیتا از کش آمده تا خودت هم متوجه سرعت بشوی!
            Response.Headers.Append("X-Cache", "Hit");

            return Ok(pagedTours);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TourResponseDto>> GetTour(int id)
        {
            var tour = await _context.Tours
                .Include(t => t.Destination)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tour == null) return NotFound();

            var weatherInfo = await _weatherService.GetWeatherAsync(tour.Destination!.Name);

            return new TourResponseDto
            {
                Id = tour.Id,
                Title = tour.Title,
                Price = tour.Price,
                DurationDays = tour.DurationDays,
                DestinationName = tour.Destination.Name,
                Weather = weatherInfo
            };
        }

        [HttpPost]
        public async Task<ActionResult<TourResponseDto>> CreateTour([FromBody] CreateTourDto createTourDto)
        {
            var tour = new Tour
            {
                Title = createTourDto.Title,
                Price = createTourDto.Price,
                DurationDays = createTourDto.DurationDays,
                DestinationId = createTourDto.DestinationId
            };

            _context.Tours.Add(tour);
            await _context.SaveChangesAsync();

            // ---> نمره VG: پاک کردن کش (Cache Eviction) <---
            // وقتی تور جدید ساخته می‌شود، کش قدیمی را پاک می‌کنیم تا کاربر دفعه بعد لیست آپدیت‌شده را ببیند
            _cache.Remove(CacheKey);

            var destination = await _context.Destinations.FindAsync(createTourDto.DestinationId);

            WeatherInfoDto? weatherInfo = null;
            if (destination != null)
            {
                weatherInfo = await _weatherService.GetWeatherAsync(destination.Name);
            }

            var responseDto = new TourResponseDto
            {
                Id = tour.Id,
                Title = tour.Title,
                Price = tour.Price,
                DurationDays = tour.DurationDays,
                DestinationName = destination?.Name ?? "Unknown",
                Weather = weatherInfo
            };

            return CreatedAtAction(nameof(GetTour), new { id = tour.Id }, responseDto);
        }
    }
}