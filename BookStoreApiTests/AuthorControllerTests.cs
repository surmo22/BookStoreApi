using AutoMapper;
using BookStoreApi.Controllers;
using BookStoreApi.Data.Repository.Interfaces;
using BookStoreApi.Models;
using BookStoreApi.Models.DTO.Author;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BookStoreApiTests
{
    public class AuthorControllerTests
    {
        private readonly Mock<IRepository<Author>> _authorRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly AuthorsController _authorsController;

        public AuthorControllerTests()
        {
            _authorRepositoryMock = new Mock<IRepository<Author>>();
            _mapperMock = new Mock<IMapper>();
            _authorsController = new AuthorsController(_authorRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetAuthors_ReturnsOkResult()
        {
            // Arrange
            var authors = new List<Author>();
            _authorRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(authors);
            _mapperMock.Setup(mapper => mapper.Map<AuthorDto>(It.IsAny<Author>())).Returns(new AuthorDto());

            // Act
            var result = await _authorsController.GetAuthors();

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetAuthors_ReturnsAllAuthors()
        {
            // Arrange
            var authors = new List<Author>
            {
                new Author { AuthorId = 1, Name = "Author 1" },
                new Author { AuthorId = 2, Name = "Author 2" }
            };
            _authorRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(authors);
            _mapperMock.Setup(mapper => mapper.Map<AuthorDto>(It.IsAny<Author>())).Returns(new AuthorDto());

            // Act
            var result = await _authorsController.GetAuthors();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var authorDtos = Assert.IsAssignableFrom<IEnumerable<AuthorDto>>(okResult.Value);
            Assert.Equal(2, authorDtos.Count());
        }

        [Fact]
        public async Task GetAuthor_WithValidId_ReturnsOkResult()
        {
            // Arrange
            var authorId = 1;
            var author = new Author { AuthorId = authorId, Name = "Author 1" };
            _authorRepositoryMock.Setup(repo => repo.GetByIdAsync(authorId)).ReturnsAsync(author);
            _mapperMock.Setup(mapper => mapper.Map<AuthorDto>(It.IsAny<Author>())).Returns(new AuthorDto { AuthorId = author.AuthorId, Name = author.Name });

            // Act
            var result = await _authorsController.GetAuthor(authorId);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetAuthor_WithValidId_ReturnsAuthorDto()
        {
            // Arrange
            var authorId = 1;
            var author = new Author { AuthorId = authorId, Name = "Author 1" };
            _authorRepositoryMock.Setup(repo => repo.GetByIdAsync(authorId)).ReturnsAsync(author);
            _mapperMock.Setup(mapper => mapper.Map<AuthorDto>(It.IsAny<Author>())).Returns(new AuthorDto { AuthorId = author.AuthorId, Name = author.Name });

            // Act
            var result = await _authorsController.GetAuthor(authorId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var authorDto = Assert.IsType<AuthorDto>(okResult.Value);
            Assert.Equal(authorId, authorDto.AuthorId);
        }

        [Fact]
        public async Task GetAuthor_WithInvalidId_ReturnsNotFoundResult()
        {
            // Arrange
            var authorId = 1;
            _authorRepositoryMock.Setup(repo => repo.GetByIdAsync(authorId)).ReturnsAsync((Author)null);

            // Act
            var result = await _authorsController.GetAuthor(authorId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task PostAuthor_WithValidAuthorDto_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var authorDto = new AuthorDto { Name = "Author 1" };
            var author = new Author { AuthorId = 1, Name = "Author 1" };
            _mapperMock.Setup(mapper => mapper.Map<Author>(authorDto)).Returns(author);
            _authorRepositoryMock.Setup(repo => repo.AddAsync(author)).Returns(Task.CompletedTask);

            // Act
            var result = await _authorsController.PostAuthor(authorDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal("GetAuthor", createdAtActionResult.ActionName);
            Assert.Equal(1, createdAtActionResult.RouteValues["id"]);
            Assert.Equal(author, createdAtActionResult.Value);
        }

        [Fact]
        public async Task PostAuthor_WithInvalidAuthorDto_ReturnsBadRequestResult()
        {
            // Arrange
            var authorDto = new AuthorDto { Name = "Author 1" };
            _authorsController.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await _authorsController.PostAuthor(authorDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task PutAuthor_WithMatchingId_ReturnsNoContentResult()
        {
            // Arrange
            var authorId = 1;
            var authorDto = new AuthorDto { AuthorId = authorId, Name = "Author 1" };
            _authorRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Author>())).Returns(Task.CompletedTask);

            // Act
            var result = await _authorsController.PutAuthor(authorId, authorDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task PutAuthor_WithMismatchingId_ReturnsBadRequestResult()
        {
            // Arrange
            var authorId = 1;
            var authorDto = new AuthorDto { AuthorId = 2, Name = "Author 1" };

            // Act
            var result = await _authorsController.PutAuthor(authorId, authorDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task PutAuthor_WithInvalidAuthorDto_ReturnsBadRequestResult()
        {
            // Arrange
            var authorId = 1;
            var authorDto = new AuthorDto { AuthorId = authorId, Name = "Author 1" };
            _authorsController.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await _authorsController.PutAuthor(authorId, authorDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeleteAuthor_WithValidId_ReturnsNoContentResult()
        {
            // Arrange
            var authorId = 1;
            var author = new Author { AuthorId = authorId, Name = "Author 1" };
            _authorRepositoryMock.Setup(repo => repo.GetByIdAsync(authorId)).ReturnsAsync(author);
            _authorRepositoryMock.Setup(repo => repo.RemoveAsync(author)).Returns(Task.CompletedTask);

            // Act
            var result = await _authorsController.DeleteAuthor(authorId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteAuthor_WithInvalidId_ReturnsNotFoundResult()
        {
            // Arrange
            var authorId = 1;
            _authorRepositoryMock.Setup(repo => repo.GetByIdAsync(authorId)).ReturnsAsync((Author)null);

            // Act
            var result = await _authorsController.DeleteAuthor(authorId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
