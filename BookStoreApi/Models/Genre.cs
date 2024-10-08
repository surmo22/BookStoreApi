﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookStoreApi.Models
{
    public class Genre
    {
        [Key]
        public int GenreId { get; set; }

        [Required(ErrorMessage = "Genre name is required.")]
        [StringLength(50, ErrorMessage = "Genre name cannot be longer than 50 characters.")]
        public string Name { get; set; }

        // Navigation property
        public ICollection<Book> Books { get; set; }
    }
}
