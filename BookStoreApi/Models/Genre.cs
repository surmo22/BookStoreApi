using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookStoreApi.Models
{
    public class Genre
    {
        [Key]
        public int GenreId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        // Navigation property
        public ICollection<Book> Books { get; set; }
    }
}
