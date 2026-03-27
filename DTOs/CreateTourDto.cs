using System.ComponentModel.DataAnnotations;

namespace TravelApi.DTOs
{
    public class CreateTourDto
    {
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Title must be between 5 and 100 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Range(100, 100000, ErrorMessage = "Price must be between 100 and 100,000.")]
        public decimal Price { get; set; }

        [Required]
        [Range(1, 30, ErrorMessage = "Duration must be between 1 and 30 days.")]
        public int DurationDays { get; set; }

        [Required(ErrorMessage = "DestinationId is required.")]
        public int DestinationId { get; set; }
    }
}