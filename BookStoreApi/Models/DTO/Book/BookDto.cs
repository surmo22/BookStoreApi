using System.ComponentModel.DataAnnotations;

namespace BookStoreApi.Models.DTO.Book
{
    public record BookDto
    {
        public int BookId { get; init; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; }

        [Range(0.01, 10000.00, ErrorMessage = "Price must be between $0.01 and $10,000")]
        public decimal Price { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int QuantityAvailable { get; set; }

        [Required]
        public int AuthorId { get; set; }

        [Required]
        public int GenreId { get; set; }
    }
}
