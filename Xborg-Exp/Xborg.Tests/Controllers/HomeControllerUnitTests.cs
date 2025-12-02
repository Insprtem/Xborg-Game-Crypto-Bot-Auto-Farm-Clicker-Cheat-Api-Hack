using DogHouse.Tests.Utillities;
using DogsHouse.Db.Entities;
using DogsHouse.Db.Repository;
using DogsHouseApp.Controllers;
using DogsHouseApp.Models;
using DogsHouseApp.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DogsHouseApp.Tests.Controllers
{
    [TestFixture]
    public class HomeControllerTests
    {
        private Mock<IDogService> _dogServiceMock;
        private HomeController _homeController;
        private DogsCollectionUtillity _dogsCollection;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _dogServiceMock = new Mock<IDogService>();

            _dogsCollection = new DogsCollectionUtillity();
        }

        [SetUp]
        public void Setup()
        {
            _dogServiceMock.Reset();

            _dogServiceMock
                .Setup(m => m.GetDogsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>()))
                .ReturnsAsync(_dogsCollection.Dogs.AsEnumerable());

            _dogServiceMock
                .Setup(mock => mock.IsDogNameAlreadyExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            _dogServiceMock
                .Setup(mock => mock.AddDogAsync(It.IsAny<DogModel>()))
                .Returns(Task.CompletedTask);

            _homeController = new HomeController(_dogServiceMock.Object);
        }

        [Test]
        public async Task Ping_ReturnOk_Success()
        {
            // Act
            var result = await _homeController.Ping();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.That(okResult.Value as string, Does.Contain("Dogs House service"));
        }

        [Test]
        public async Task Dogs_ValidSort_ReturnSortedDogs()
        {
            // Arrange
            var dogs = _dogsCollection.Dogs;

            // Act
            var result = await _homeController.Dogs(attribute: "name", order: "asc");

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.That(okResult.Value, Is.EqualTo(dogs));

            _dogServiceMock.Verify(mock => mock.GetDogsAsync("name", "asc", null, null), Times.Once);
            _dogServiceMock.Verify(mock => mock.GetDogsAsync(It.IsNotIn("name"), It.IsNotIn("asc"), It.IsAny<int?>(), It.IsAny<int?>()), Times.Never);
        }

        [Test]
        public async Task Dogs_ValidPagination_ReturnDogsByPage()
        {
            // Arrange
            var dogs = _dogsCollection.Dogs;

            // Act
            var result = await _homeController.Dogs(pageNumber: 2, pageSize: 2);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.That(okResult.Value, Is.EqualTo(dogs));

            _dogServiceMock.Verify(mock => mock.GetDogsAsync(null, null, 2, 2), Times.Once);
            _dogServiceMock.Verify(mock => mock.GetDogsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsNotIn(2), It.IsNotIn(2)), Times.Never);
        }

        [Test]
        public async Task Dogs_Invalid_Exception()
        {
            var exceptionMessage = "Test Exception";

            _dogServiceMock
                .Setup(m => m.GetDogsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>()))
                .Throws(new Exception(exceptionMessage));

            // Act
            var result = await _homeController.Dogs();

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);

            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult); 
            Assert.That(badRequestResult.Value, Is.EqualTo(exceptionMessage));

            _dogServiceMock.Verify(mock => mock.GetDogsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>()), Times.Once);
        }

        [Test]
        public async Task Dog_NameIsAlreadyExist_ReturnBadRequest()
        {
            // Arrange
            var dogModel = new DogModel
            {
                Name = _dogsCollection.Dog1.Name,
                Color = _dogsCollection.Dog1.Color,
                TailLength = _dogsCollection.Dog1.TailLength,
                Weight = _dogsCollection.Dog1.Weight
            };

            _dogServiceMock.Setup(mock => mock.IsDogNameAlreadyExistsAsync(dogModel.Name)).ReturnsAsync(true);
            _dogServiceMock.Setup(mock => mock.IsDogNameAlreadyExistsAsync(It.IsNotIn(dogModel.Name))).ReturnsAsync(false);

            // Act
            var result = await _homeController.Dog(dogModel);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);

            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.That(badRequestResult.Value, Is.EqualTo($"Dog with name {dogModel.Name} is already exist!"));

            _dogServiceMock.Verify(mock => mock.IsDogNameAlreadyExistsAsync(dogModel.Name), Times.Once);
            _dogServiceMock.Verify(mock => mock.IsDogNameAlreadyExistsAsync(It.IsNotIn(dogModel.Name)), Times.Never);
            _dogServiceMock.Verify(mock => mock.AddDogAsync(It.IsAny<DogModel>()), Times.Never);
        }

        [Test]
        public async Task Dog_InvalidWeight_ReturnBadRequest()
        {
            // Arrange
            var dogModel = new DogModel 
            { 
                Name = "NewName", 
                Color = "NewColor", 
                TailLength = 10, 
                Weight = -5 
            };

            // Act
            var result = await _homeController.Dog(dogModel);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);

            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.That(badRequestResult.Value, Is.EqualTo($"Weight is not valid"));

            _dogServiceMock.Verify(mock => mock.IsDogNameAlreadyExistsAsync(dogModel.Name), Times.Once);
            _dogServiceMock.Verify(mock => mock.AddDogAsync(It.IsAny<DogModel>()), Times.Never);
        }

        [Test]
        public async Task Dog_InvalidTailLength_ReturnBadRequest()
        {
            // Arrange
            var dogModel = new DogModel
            {
                Name = "NewName",
                Color = "NewColor",
                TailLength = -10,
                Weight = 5
            };

            // Act
            var result = await _homeController.Dog(dogModel);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);

            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.That(badRequestResult.Value, Is.EqualTo($"Tail length is not valid"));

            _dogServiceMock.Verify(mock => mock.IsDogNameAlreadyExistsAsync(dogModel.Name), Times.Once);
            _dogServiceMock.Verify(mock => mock.AddDogAsync(It.IsAny<DogModel>()), Times.Never);
        }

        [Test]
        public async Task Dog_Valid_Success()
        {
            // Arrange
            var dogModel = new DogModel 
            { 
                Name = "Rex", 
                Color = "Brown", 
                TailLength = 10, 
                Weight = 25 
            };

            // Act
            var result = await _homeController.Dog(dogModel);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.That(okResult.Value, Is.EqualTo("Created"));

            _dogServiceMock.Verify(mock => mock.IsDogNameAlreadyExistsAsync(dogModel.Name), Times.Once);
            _dogServiceMock.Verify(mock => mock.AddDogAsync(dogModel), Times.Once);
        }
    }
}
