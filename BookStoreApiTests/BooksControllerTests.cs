using AutoMapper;
using BookStoreApi.Controllers;
using BookStoreApi.Data.Repository.Interfaces;
using BookStoreApi.Models;
using BookStoreApi.Models.DTO.Author;
using BookStoreApi.Models.DTO.Book;
using BookStoreApi.Models.DTO.Genre;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BookStoreApiTests
{
    public class BooksControllerTests
    {
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        private readonly Mock<IRepository<Author>> _authorRepositoryMock;
        private readonly Mock<IRepository<Genre>> _genreRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly BooksController _booksController;

        public BooksControllerTests()
        {
            _bookRepositoryMock = new Mock<IBookRepository>();
            _authorRepositoryMock = new Mock<IRepository<Author>>();
            _genreRepositoryMock = new Mock<IRepository<Genre>>();
            _mapperMock = new Mock<IMapper>();
            _booksController = new BooksController(_bookRepositoryMock.Object, _authorRepositoryMock.Object, _genreRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetBooks_ReturnsOkResult()
        {
            // Arrange
            var books = new List<Book>();
            _bookRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(books);
            _mapperMock.Setup(mapper => mapper.Map<BookDto>(It.IsAny<Book>())).Returns(new BookDto());

            // Act
            var result = await _booksController.GetBooks();

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetBooks_ReturnsAllBooks()
        {
            // Arrange
            var books = new List<Book>
            {
                new Book { BookId = 1, Title = "Book 1" },
                new Book { BookId = 2, Title = "Book 2" }
            };
            _bookRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(books);
            _mapperMock.Setup(mapper => mapper.Map<BookDto>(It.IsAny<Book>())).Returns(new BookDto());

            // Act
            var result = await _booksController.GetBooks();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var bookDtos = Assert.IsAssignableFrom<IEnumerable<BookDto>>(okResult.Value);
            Assert.Equal(2, bookDtos.Count());
        }

        [Fact]
        public async Task GetBook_WithValidId_ReturnsOkResult()
        {
            // Arrange
            var bookId = 1;
            var book = new Book { BookId = bookId, Title = "Book 1" };
            _bookRepositoryMock.Setup(repo => repo.GetByIdAsync(bookId)).ReturnsAsync(book);
            _mapperMock.Setup(mapper => mapper.Map<BookDto>(It.IsAny<Book>())).Returns(new BookDto { BookId = book.BookId, Title = book.Title });

            // Act
            var result = await _booksController.GetBook(bookId);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetBook_WithValidId_ReturnsBookDto()
        {
            // Arrange
            var bookId = 1;
            var book = new Book { BookId = bookId, Title = "Book 1" };
            _bookRepositoryMock.Setup(repo => repo.GetByIdAsync(bookId)).ReturnsAsync(book);
            _mapperMock.Setup(mapper => mapper.Map<BookDto>(It.IsAny<Book>())).Returns(new BookDto { BookId = book.BookId, Title = book.Title });

            // Act
            var result = await _booksController.GetBook(bookId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var bookDto = Assert.IsType<BookDto>(okResult.Value);
            Assert.Equal(bookId, bookDto.BookId);
        }

        [Fact]
        public async Task GetBook_WithInvalidId_ReturnsNotFoundResult()
        {
            // Arrange
            var bookId = 1;
            _bookRepositoryMock.Setup(repo => repo.GetByIdAsync(bookId)).ReturnsAsync((Book)null);

            // Act
            var result = await _booksController.GetBook(bookId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task PostBook_WithValidBookDto_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var bookDto = new BookDto { Title = "Book 1", AuthorId = 1, GenreId = 1 };
            var book = new Book { BookId = 1, Title = "Book 1", AuthorId = 1, GenreId = 1 };
            _authorRepositoryMock.Setup(repo => repo.GetByIdAsync(bookDto.AuthorId)).ReturnsAsync(new Author());
            _genreRepositoryMock.Setup(repo => repo.GetByIdAsync(bookDto.GenreId)).ReturnsAsync(new Genre());
            _mapperMock.Setup(mapper => mapper.Map<Book>(bookDto)).Returns(book);
            _bookRepositoryMock.Setup(repo => repo.AddAsync(book)).Returns(Task.CompletedTask);

            // Act
            var result = await _booksController.PostBook(bookDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal("GetBook", createdAtActionResult.ActionName);
            Assert.Equal(1, createdAtActionResult.RouteValues["id"]);
            Assert.Equal(bookDto, createdAtActionResult.Value);
        }

        [Fact]
        public async Task PostBook_WithInvalidBookDto_ReturnsBadRequestResult()
        {
            // Arrange
            var bookDto = new BookDto { Title = "Book 1", AuthorId = 1, GenreId = 1 };
            _booksController.ModelState.AddModelError("Title", "Title is required");

            // Act
            var result = await _booksController.PostBook(bookDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task PostBook_WithInvalidAuthorId_ReturnsBadRequestResult()
        {
            // Arrange
            var bookDto = new BookDto { Title = "Book 1", AuthorId = 1, GenreId = 1 };
            _authorRepositoryMock.Setup(repo => repo.GetByIdAsync(bookDto.AuthorId)).ReturnsAsync((Author)null);

            // Act
            var result = await _booksController.PostBook(bookDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Invalid AuthorId or GenreId", badRequestResult.Value);
        }

        [Fact]
        public async Task PostBook_WithInvalidGenreId_ReturnsBadRequestResult()
        {
            // Arrange
            var bookDto = new BookDto { Title = "Book 1", AuthorId = 1, GenreId = 1 };
            _authorRepositoryMock.Setup(repo => repo.GetByIdAsync(bookDto.AuthorId)).ReturnsAsync(new Author());
            _genreRepositoryMock.Setup(repo => repo.GetByIdAsync(bookDto.GenreId)).ReturnsAsync((Genre)null);

            // Act
            var result = await _booksController.PostBook(bookDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Invalid AuthorId or GenreId", badRequestResult.Value);
        }

        [Fact]
        public async Task PutBook_WithMatchingId_ReturnsNoContentResult()
        {
            // Arrange
            var bookId = 1;
            var bookDto = new BookDto { BookId = bookId, Title = "Book 1", AuthorId = 1, GenreId = 1 };
            _bookRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Book>())).Returns(Task.CompletedTask);

            // Act
            var result = await _booksController.PutBook(bookId, bookDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task PutBook_WithMismatchingId_ReturnsBadRequestResult()
        {
            // Arrange
            var bookId = 1;
            var bookDto = new BookDto { BookId = 2, Title = "Book 1", AuthorId = 1, GenreId = 1 };

            // Act
            var result = await _booksController.PutBook(bookId, bookDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Book ID mismatch", badRequestResult.Value);
        }

        [Fact]
        public async Task PutBook_WithInvalidBookDto_ReturnsBadRequestResult()
        {
            // Arrange
            var bookId = 1;
            var bookDto = new BookDto { BookId = bookId, Title = "Book 1", AuthorId = 1, GenreId = 1 };
            _booksController.ModelState.AddModelError("Title", "Title is required");

            // Act
            var result = await _booksController.PutBook(bookId, bookDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeleteBook_WithValidId_ReturnsNoContentResult()
        {
            // Arrange
            var bookId = 1;
            var book = new Book { BookId = bookId, Title = "Book 1" };
            _bookRepositoryMock.Setup(repo => repo.GetByIdAsync(bookId)).ReturnsAsync(book);
            _bookRepositoryMock.Setup(repo => repo.RemoveAsync(book)).Returns(Task.CompletedTask);

            // Act
            var result = await _booksController.DeleteBook(bookId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteBook_WithInvalidId_ReturnsNotFoundResult()
        {
            // Arrange
            var bookId = 1;
            _bookRepositoryMock.Setup(repo => repo.GetByIdAsync(bookId)).ReturnsAsync((Book)null);

            // Act
            var result = await _booksController.DeleteBook(bookId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task SearchBooks_WithValidParameters_ReturnsOkResult()
        {
            // Arrange
            var title = "Book";
            var author = "Author";
            var genre = "Genre";
            var books = new List<Book>
            {
                new Book { BookId = 1, Title = "Book 1", Author = new Author { Name = "Author 1" }, Genre = new Genre { Name = "Genre 1" } },
                new Book { BookId = 2, Title = "Book 2", Author = new Author { Name = "Author 2" }, Genre = new Genre { Name = "Genre 2" } }
            };
            _bookRepositoryMock.Setup(repo => repo.SearchBooksAsync(title, author, genre)).ReturnsAsync(books);
            _mapperMock.Setup(mapper => mapper.Map<BookDto>(It.IsAny<Book>())).Returns(new BookDto());

            // Act
            var result = await _booksController.SearchBooks(title, author, genre);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task SearchBooks_WithValidParameters_ReturnsMatchingBookDtos()
        {
            // Arrange
            var title = "Book";
            var author = "Author";
            var genre = "Genre";
            var books = new List<Book>
            {
                new Book { BookId = 1, Title = "Book 1", Author = new Author { Name = "Author 1" }, Genre = new Genre { Name = "Genre 1" } },
                new Book { BookId = 2, Title = "Book 2", Author = new Author { Name = "Author 2" }, Genre = new Genre { Name = "Genre 2" } }
            };
            _bookRepositoryMock.Setup(repo => repo.SearchBooksAsync(title, author, genre)).ReturnsAsync(books);
            _mapperMock.Setup(mapper => mapper.Map<BookDto>(It.IsAny<Book>())).Returns(new BookDto());

            // Act
            var result = await _booksController.SearchBooks(title, author, genre);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var bookDtos = Assert.IsAssignableFrom<IEnumerable<BookDto>>(okResult.Value);
            Assert.Equal(2, bookDtos.Count());
        }

        [Fact]
        public async Task SearchBooks_WithNoMatchingBooks_ReturnsNotFoundResult()
        {
            // Arrange
            var title = "Book";
            var author = "Author";
            var genre = "Genre";
            var books = new List<Book>();
            _bookRepositoryMock.Setup(repo => repo.SearchBooksAsync(title, author, genre)).ReturnsAsync(books);

            // Act
            var result = await _booksController.SearchBooks(title, author, genre);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }
    }
}
