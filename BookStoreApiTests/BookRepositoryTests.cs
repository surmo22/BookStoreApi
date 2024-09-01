using AutoMapper;
using BookStoreApi.Data;
using BookStoreApi.Data.Repository.Implementations;
using BookStoreApi.Data.Repository.Interfaces;
using BookStoreApi.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BookStoreApiTests
{
    public class BookRepositoryTests
    {
        private readonly Mock<ApplicationDbContext> _contextMock;
        private readonly Mock<DbSet<Book>> _bookDbSetMock;
        private readonly BookRepository _bookRepository;
        private readonly List<Book> _data;

        public BookRepositoryTests()
        {
            _contextMock = new Mock<ApplicationDbContext>();
            _data = new List<Book>
        {
            new Book { BookId = 1, Title = "Harry Potter", Author = new Author { Name = "J.K. Rowling" }, Genre = new Genre { Name = "Fantasy" }},
            new Book { BookId = 2, Title = "Lord of the Rings", Author = new Author { Name = "J.R.R. Tolkien" }, Genre = new Genre { Name = "Fantasy" }}
        };

            _bookDbSetMock = _data.AsAsyncDbSetMock();
            _contextMock.Setup(c => c.Books).Returns(_bookDbSetMock.Object);
            _bookRepository = new BookRepository(_contextMock.Object);
        }

        [Theory]
        [InlineData("Harry", null, null)]
        [InlineData(null, "Tolkien", null)]
        [InlineData(null, null, "Fantasy")]
        public async Task SearchBooksAsync_ReturnsFilteredBooks(string title, string author, string genre)
        {
            // Act
            var result = await _bookRepository.SearchBooksAsync(title, author, genre);

            // Assert
            Assert.All(result, book =>
            {
                if (!string.IsNullOrWhiteSpace(title))
                    Assert.Contains(title, book.Title);
                if (!string.IsNullOrWhiteSpace(author))
                    Assert.Contains(author, book.Author.Name);
                if (!string.IsNullOrWhiteSpace(genre))
                    Assert.Contains(genre, book.Genre.Name);
            });
        }

        [Fact]
        public async Task SearchBooksAsync_WithoutFilters_ReturnsAllBooks()
        {
            // Act
            var result = await _bookRepository.SearchBooksAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(_data[0], result);
            Assert.Contains(_data[1], result);
        }
    }
}
