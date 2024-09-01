using BookStoreApi.Models;

namespace BookStoreApi.Data.Repository.Interfaces
{
    public interface IBookRepository : IRepository<Book>
    {
        Task<IEnumerable<Book>> SearchBooksAsync(string? bookTitle = null, string? authorName = null, string? genreName = null);
    }
}
