using DogHouse.Tests.Utillities;
using DogsHouse.Db;
using DogsHouse.Db.Entities;
using DogsHouse.Db.Repository;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace DogHouse.Tests.Repository
{
    [TestFixture]
    public class RepositoryUnitTests
    {
        private Mock<DogsHouseDbContext> _mockDogsHouseDbContext;
        private Mock<DbSet<Dog>> _mockDbSet;

        private IRepository<Dog> _repository;

        private DogsCollectionUtillity _dogsCollection;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _dogsCollection = new DogsCollectionUtillity();

            _mockDbSet = MockUtillity.GetMockedDbSet(_dogsCollection.Dogs);
            _mockDogsHouseDbContext = new Mock<DogsHouseDbContext>();

            _mockDogsHouseDbContext.Setup(c => c.Set<Dog>()).Returns(_mockDbSet.Object);

            _repository = new Repository<Dog>(_mockDogsHouseDbContext.Object);
        }

        [SetUp]
        public void SetUp()
        {
            _mockDbSet.Invocations.Clear();
            _mockDogsHouseDbContext.Invocations.Clear();
        }

        [Test]
        public async Task AddAsync_Valid_Success()
        {
            // Arrange
            var entity = new Dog();

            // Act
            await _repository.AddAsync(entity);

            // Assert
            _mockDbSet.Verify(d => d.AddAsync(entity, default), Times.Once);
            _mockDogsHouseDbContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }
    }
}
