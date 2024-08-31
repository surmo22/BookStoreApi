using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace BookStoreApi.Models
{
    

    public class Author
    {
        [Key]
        public int AuthorId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        // Navigation property
        public ICollection<Book> Books { get; set; }
    }
}
