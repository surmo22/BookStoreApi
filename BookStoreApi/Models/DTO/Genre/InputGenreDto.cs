using System.ComponentModel.DataAnnotations;

namespace BookStoreApi.Models.DTO.Genre
{
    public class InputGenreDto
    {
        public int GenreId { get; set; }

        [Required(ErrorMessage = "Genre name is required.")]
        [StringLength(50, ErrorMessage = "Genre name cannot be longer than 50 characters.")]
        public string Name { get; set; }
    }
}
