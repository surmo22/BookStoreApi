using AutoMapper;
using BookStoreApi.Controllers;
using BookStoreApi.Data.Repository.Interfaces;
using BookStoreApi.Models;
using BookStoreApi.Models.DTO.Genre;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BookStoreApiTests
{
    public class GenreControllerTests
    {
        private readonly Mock<IRepository<Genre>> _genreRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GenresController _genresController;

        public GenreControllerTests()
        {
            _genreRepositoryMock = new Mock<IRepository<Genre>>();
            _mapperMock = new Mock<IMapper>();
            _genresController = new GenresController(_genreRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetGenres_ReturnsOkResult()
        {
            // Arrange
            var genres = new List<Genre>();
            _genreRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(genres);
            _mapperMock.Setup(mapper => mapper.Map<GenreDto>(It.IsAny<Genre>())).Returns(new GenreDto());

            // Act
            var result = await _genresController.GetGenres();

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetGenres_ReturnsAllGenres()
        {
            // Arrange
            var genres = new List<Genre>
            {
                new Genre { GenreId = 1, Name = "Genre 1" },
                new Genre { GenreId = 2, Name = "Genre 2" }
            };
            _genreRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(genres);
            _mapperMock.Setup(mapper => mapper.Map<GenreDto>(It.IsAny<Genre>())).Returns(new GenreDto());

            // Act
            var result = await _genresController.GetGenres();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var genreDtos = Assert.IsAssignableFrom<IEnumerable<GenreDto>>(okResult.Value);
            Assert.Equal(2, genreDtos.Count());
        }

        [Fact]
        public async Task GetGenre_WithValidId_ReturnsOkResult()
        {
            // Arrange
            var genreId = 1;
            var genre = new Genre { GenreId = genreId, Name = "Genre 1" };
            _genreRepositoryMock.Setup(repo => repo.GetByIdAsync(genreId)).ReturnsAsync(genre);
            _mapperMock.Setup(mapper => mapper.Map<GenreDto>(It.IsAny<Genre>())).Returns(new GenreDto { GenreId = genre.GenreId, Name = genre.Name });

            // Act
            var result = await _genresController.GetGenre(genreId);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetGenre_WithValidId_ReturnsGenreDto()
        {
            // Arrange
            var genreId = 1;
            var genre = new Genre { GenreId = genreId, Name = "Genre 1" };
            _genreRepositoryMock.Setup(repo => repo.GetByIdAsync(genreId)).ReturnsAsync(genre);
            _mapperMock.Setup(mapper => mapper.Map<GenreDto>(It.IsAny<Genre>())).Returns(new GenreDto { GenreId = genre.GenreId, Name = genre.Name });

            // Act
            var result = await _genresController.GetGenre(genreId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var genreDto = Assert.IsType<GenreDto>(okResult.Value);
            Assert.Equal(genreId, genreDto.GenreId);
        }

        [Fact]
        public async Task GetGenre_WithInvalidId_ReturnsNotFoundResult()
        {
            // Arrange
            var genreId = 1;
            _genreRepositoryMock.Setup(repo => repo.GetByIdAsync(genreId)).ReturnsAsync((Genre)null);

            // Act
            var result = await _genresController.GetGenre(genreId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task PostGenre_WithValidGenreDto_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var genreDto = new GenreDto { Name = "Genre 1" };
            var genre = new Genre { GenreId = 1, Name = "Genre 1" };
            _mapperMock.Setup(mapper => mapper.Map<Genre>(genreDto)).Returns(genre);
            _genreRepositoryMock.Setup(repo => repo.AddAsync(genre)).Returns(Task.CompletedTask);

            // Act
            var result = await _genresController.PostGenre(genreDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal("GetGenre", createdAtActionResult.ActionName);
            Assert.Equal(1, createdAtActionResult.RouteValues["id"]);
            Assert.Equal(genre, createdAtActionResult.Value);
        }

        [Fact]
        public async Task PostGenre_WithInvalidGenreDto_ReturnsBadRequestResult()
        {
            // Arrange
            var genreDto = new GenreDto { Name = "Genre 1" };
            _genresController.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await _genresController.PostGenre(genreDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task PutGenre_WithMatchingId_ReturnsNoContentResult()
        {
            // Arrange
            var genreId = 1;
            var genreDto = new GenreDto { GenreId = genreId, Name = "Genre 1" };
            _genreRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Genre>())).Returns(Task.CompletedTask);

            // Act
            var result = await _genresController.PutGenre(genreId, genreDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task PutGenre_WithMismatchingId_ReturnsBadRequestResult()
        {
            // Arrange
            var genreId = 1;
            var genreDto = new GenreDto { GenreId = 2, Name = "Genre 1" };

            // Act
            var result = await _genresController.PutGenre(genreId, genreDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task PutGenre_WithInvalidGenreDto_ReturnsBadRequestResult()
        {
            // Arrange
            var genreId = 1;
            var genreDto = new GenreDto { GenreId = genreId, Name = "Genre 1" };
            _genresController.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await _genresController.PutGenre(genreId, genreDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeleteGenre_WithValidId_ReturnsNoContentResult()
        {
            // Arrange
            var genreId = 1;
            var genre = new Genre { GenreId = genreId, Name = "Genre 1" };
            _genreRepositoryMock.Setup(repo => repo.GetByIdAsync(genreId)).ReturnsAsync(genre);
            _genreRepositoryMock.Setup(repo => repo.RemoveAsync(genre)).Returns(Task.CompletedTask);

            // Act
            var result = await _genresController.DeleteGenre(genreId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteGenre_WithInvalidId_ReturnsNotFoundResult()
        {
            // Arrange
            var genreId = 1;
            _genreRepositoryMock.Setup(repo => repo.GetByIdAsync(genreId)).ReturnsAsync((Genre)null);

            // Act
            var result = await _genresController.DeleteGenre(genreId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
