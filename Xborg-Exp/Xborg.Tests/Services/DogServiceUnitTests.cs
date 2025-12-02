using DogHouse.Tests.Utillities;
using DogsHouse.Db.Entities;
using DogsHouse.Db.Repository;
using DogsHouseApp.Models;
using DogsHouseApp.Services;
using Moq;

namespace DogHouse.Tests.Services
{
    [TestFixture]
    public class DogServiceUnitTests
    {
        private Mock<IRepository<Dog>> _repositoryMock;
        private DogService _dogService;

        private DogsCollectionUtillity _dogsCollection;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _repositoryMock = new Mock<IRepository<Dog>>();

            _dogsCollection = new DogsCollectionUtillity();
        }

        [SetUp]
        public void Setup()
        {
            _repositoryMock.Reset();

            _repositoryMock.Setup(m => m.GetAllAsync()).ReturnsAsync(_dogsCollection.Dogs.AsEnumerable());

            _dogService = new DogService(_repositoryMock.Object);
        }

        [Test]
        public async Task AddDogAsync_Valid_Success()
        {
            // Arrange
            var dogModel = new DogModel 
            { 
                Name = "NewDog", 
                Color = "Brown",
                TailLength = 5.3, 
                Weight = 25 
            };

            var expectedDog = new Dog 
            { 
                Name = dogModel.Name, 
                Color = dogModel.Color, 
                TailLength = dogModel.TailLength, 
                Weight = dogModel.Weight
            };

            // Act
            await _dogService.AddDogAsync(dogModel);

            // Assert
            _repositoryMock.Verify(mock => mock.GetAllAsync(), Times.Once);

            _repositoryMock.Verify(mock => mock.AddAsync(It.Is<Dog>(dog => 
                dog.Name == expectedDog.Name 
                && dog.Color == expectedDog.Color
                && dog.TailLength == expectedDog.TailLength 
                && dog.Weight == expectedDog.Weight)), Times.Once);
        }

        [Test]
        public async Task AddDogAsync_AlreadyExist_Exception()
        {
            // Arrange
            var dogModel = new DogModel
            {
                Name = _dogsCollection.Dog1.Name,
                Color = "Brown",
                TailLength = 5.3,
                Weight = 25
            };

            // Act
            var result = Assert.ThrowsAsync<ArgumentException>(async () => await _dogService.AddDogAsync(dogModel));

            // Assert
            Assert.That(result.Message, Does.Contain($"Dog with name {dogModel.Name} is already exist"));

            _repositoryMock.Verify(mock => mock.GetAllAsync(), Times.Once);
            _repositoryMock.Verify(mock => mock.AddAsync(It.IsAny<Dog>()), Times.Never);
        }

        [Test]
        public async Task AddDogAsync_InvalidTailLength_Exception()
        {
            // Arrange
            var dogModel = new DogModel
            {
                Name = "NewName",
                Color = "Brown",
                TailLength = -2,
                Weight = 25
            };

            // Act
            var result = Assert.ThrowsAsync<ArgumentException>(async () => await _dogService.AddDogAsync(dogModel));

            // Assert
            Assert.That(result.Message, Does.Contain($"TailLength is not valid"));

            _repositoryMock.Verify(mock => mock.GetAllAsync(), Times.Once);
            _repositoryMock.Verify(mock => mock.AddAsync(It.IsAny<Dog>()), Times.Never);
        }

        [Test]
        public async Task AddDogAsync_InvalidWeight_Exception()
        {
            // Arrange
            var dogModel = new DogModel
            {
                Name = "NewName",
                Color = "Brown",
                TailLength = 10,
                Weight = -25
            };

            // Act
            var result = Assert.ThrowsAsync<ArgumentException>(async () => await _dogService.AddDogAsync(dogModel));

            // Assert
            Assert.That(result.Message, Does.Contain($"Weight is not valid"));

            _repositoryMock.Verify(mock => mock.GetAllAsync(), Times.Once);
            _repositoryMock.Verify(mock => mock.AddAsync(It.IsAny<Dog>()), Times.Never);
        }

        [Test]
        public async Task GetDogsAsync_ValidPagination_ReturnPaginatedDogs()
        {
            // Arrange
            var dogs = _dogsCollection.Dogs;
            var pageNumber = 2;
            var pageSize = 1;
            var expectedDogs = dogs.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            // Act
            var result = await _dogService.GetDogsAsync(null, null, pageNumber, pageSize);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);
            CollectionAssert.AreEqual(expectedDogs, result);

            _repositoryMock.Verify(mock => mock.GetAllAsync(), Times.Once);
        }

        [Test]
        public async Task GetDogsAsync_ValidSortingAsc_ReturSortedDogs()
        {
            // Arrange
            var dogs = _dogsCollection.Dogs;
            var attribute = "name";
            var orderAsc = "asc";
            var expectedSortedDogsAsc = dogs.OrderBy(dog => dog.Name);

            // Act
            var resultAsc = await _dogService.GetDogsAsync(attribute, orderAsc);

            // Assert
            Assert.IsNotNull(resultAsc);
            Assert.IsNotEmpty(resultAsc);
            CollectionAssert.AreEqual(expectedSortedDogsAsc, resultAsc);

            _repositoryMock.Verify(mock => mock.GetAllAsync(), Times.Once);
        }

        [Test]
        public async Task GetDogsAsync_ValidSortingDesc_ReturSortedDogs()
        {
            // Arrange
            var dogs = _dogsCollection.Dogs;
            var attribute = "name";
            var orderDesc = "desc";
            var expectedSortedDogsDesc = dogs.OrderByDescending(dog => dog.Name);

            // Act
            var resultDesc = await _dogService.GetDogsAsync(attribute, orderDesc);

            // Assert
            Assert.IsNotNull(resultDesc);
            Assert.IsNotEmpty(resultDesc);
            CollectionAssert.AreEqual(expectedSortedDogsDesc, resultDesc);

            _repositoryMock.Verify(mock => mock.GetAllAsync(), Times.Once);
        }


        [Test]
        public async Task GetDogsAsync_Valid_ReturPaginatedAndSortedDogs()
        {
            // Arrange
            var dogs = _dogsCollection.Dogs;
            var pageNumber = 2;
            var pageSize = 1;

            var attribute = "name";
            var orderDesc = "desc";

            var expectedSortedDogsDesc = dogs.OrderByDescending(dog => dog.Name).AsEnumerable();
            expectedSortedDogsDesc = expectedSortedDogsDesc.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            // Act
            var resultDesc = await _dogService.GetDogsAsync(attribute, orderDesc, pageNumber, pageSize);

            // Assert
            Assert.IsNotNull(resultDesc);
            Assert.IsNotEmpty(resultDesc);
            CollectionAssert.AreEqual(expectedSortedDogsDesc, resultDesc);

            _repositoryMock.Verify(mock => mock.GetAllAsync(), Times.Once);
        }

        [Test]
        public async Task GetDogsAsync_Empty_ReturAllDogs()
        {
            // Arrange
            var dogs = _dogsCollection.Dogs;

            // Act
            var result = await _dogService.GetDogsAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);
            CollectionAssert.AreEqual(dogs, result);

            _repositoryMock.Verify(mock => mock.GetAllAsync(), Times.Once);
        }

        [Test]
        public async Task IsDogNameAlreadyExistsAsync_NameExists_ReturnsTrue()
        {
            // Arrange
            var name = _dogsCollection.Dog1.Name;

            // Act
            var result = await _dogService.IsDogNameAlreadyExistsAsync(name);

            // Assert
            Assert.IsTrue(result);

            _repositoryMock.Verify(mock => mock.GetAllAsync(), Times.Once);
        }

        [Test]
        public async Task IsDogNameAlreadyExistsAsync_NameNotExist_ReturnsFalse()
        {
            // Arrange
            var name = "NewName";

            // Act
            var result = await _dogService.IsDogNameAlreadyExistsAsync(name);

            // Assert
            Assert.IsFalse(result);

            _repositoryMock.Verify(mock => mock.GetAllAsync(), Times.Once);
        }
    }
}
