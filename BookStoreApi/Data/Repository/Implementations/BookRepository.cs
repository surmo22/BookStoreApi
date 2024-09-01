using BookStoreApi.Data.Repository.Interfaces;
using BookStoreApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApi.Data.Repository.Implementations
{
    public class BookRepository : Repository<Book>, IBookRepository
    {
        public BookRepository(ApplicationDbContext context) : base(context)
        {
        }


        public async Task<IEnumerable<Book>> SearchBooksAsync(string? bookTitle = null, string? authorName = null, string? genreName = null)
        {
            IEnumerable<Book> query = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .ToListAsync();

            if (!string.IsNullOrWhiteSpace(bookTitle))
            {
                query = query.Where(b => b.Title.Contains(bookTitle));
            }

            if (!string.IsNullOrWhiteSpace(authorName))
            {
                query = query.Where(b => b.Author.Name.Contains(authorName));
            }

            if (!string.IsNullOrWhiteSpace(genreName))
            {
                query = query.Where(b => b.Genre.Name.Contains(genreName));
            }

            return query;
        }
    }
}
