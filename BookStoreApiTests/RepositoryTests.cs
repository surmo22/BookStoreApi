using BookStoreApi.Data;
using BookStoreApi.Data.Repository.Implementations;
using BookStoreApi.Data.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace BookStoreApiTests
{
    public class RepositoryTests
    {
        private readonly Mock<ApplicationDbContext> _contextMock;
        private readonly Mock<DbSet<TestEntity>> _dbSetMock;
        private readonly Repository<TestEntity> _repository;
        private readonly List<TestEntity> _data;

        public RepositoryTests()
        {
            _contextMock = new Mock<ApplicationDbContext>();
            _data = new List<TestEntity> { new TestEntity { Id = 1, Name = "Entity 1" }, new TestEntity { Id = 2, Name = "Entity2" } };
            _dbSetMock = _data.AsAsyncDbSetMock();

            _contextMock.Setup(c => c.Set<TestEntity>()).Returns(_dbSetMock.Object);
            _repository = new Repository<TestEntity>(_contextMock.Object);
        }

        [Fact]
        public async Task AddAsync_ShouldAddEntityToContext()
        {
            // Arrange
            var entity = new TestEntity();

            // Act
            await _repository.AddAsync(entity);

            // Assert
            _dbSetMock.Verify(d => d.AddAsync(entity, default), Times.Once);
        }

        [Fact]
        public async Task AddAsync_ShouldSaveChanges()
        {
            // Arrange
            var entity = new TestEntity();

            // Act
            await _repository.AddAsync(entity);

            // Assert
            _contextMock.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }


        [Fact]
        public async Task GetAllAsync_ShouldReturnAllEntities()
        {
            // Arrange       

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.Equal(_data, result);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnMatchingEntity()
        {
            // Arrange
            var entity = new TestEntity { Id = 1, Name = "Entity 1" };

            _dbSetMock.Setup(d => d.FindAsync(entity.Id)).ReturnsAsync(entity);

            // Act
            var result = await _repository.GetByIdAsync(entity.Id);

            // Assert
            Assert.Equal(entity, result);
        }

        [Fact]
        public async Task RemoveAsync_ShouldRemoveEntityFromContext()
        {
            // Arrange
            var entity = new TestEntity();

            // Act
            await _repository.RemoveAsync(entity);

            // Assert
            _dbSetMock.Verify(d => d.Remove(entity), Times.Once);
        }

        [Fact]
        public async Task RemoveAsync_ShouldSaveChanges()
        {
            // Arrange
            var entity = new TestEntity();

            // Act
            await _repository.RemoveAsync(entity);

            // Assert
            _contextMock.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateEntityInContext()
        {
            // Arrange
            var entity = new TestEntity();

            // Act
            await _repository.UpdateAsync(entity);

            // Assert
            _dbSetMock.Verify(d => d.Update(entity), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldSaveChanges()
        {
            // Arrange
            var entity = new TestEntity();

            // Act
            await _repository.UpdateAsync(entity);

            // Assert
            _contextMock.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }
    }

    public class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
