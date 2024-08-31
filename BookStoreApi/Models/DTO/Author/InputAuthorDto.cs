using System.ComponentModel.DataAnnotations;

namespace BookStoreApi.Models.DTO.Author
{
    public record InputAuthorDto
    {
        public int AuthorId { get; init; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string Name { get; init; }
    }
}
