using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace BookStoreApi.Models
{
    

    public class Author
    {
        [Key]
        public int AuthorId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string Name { get; set; }

        // Navigation property
        public ICollection<Book> Books { get; set; }
    }
}
