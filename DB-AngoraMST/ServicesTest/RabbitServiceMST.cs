using DB_AngoraLib.MockData;
using DB_AngoraLib.Models;
using DB_AngoraLib.Repository;
using DB_AngoraLib.Services.RabbitService;
using DB_AngoraLib.Services.UserService;
using DB_AngoraLib.Services.ValidationService;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraMST.ServicesTest
{
    [TestClass]
    public class RabbitServiceMST
    {
        private RabbitService _rabbitService;
        private Mock<IGRepository<Rabbit>> _mockRepository;
        private Mock<IUserService> _mockUserService;


        [TestInitialize]
        public void Setup() // Bemærk: Rækkefølgen af disse metoder er vigtig
        {
            // Opret et mock repository
            _mockRepository = new Mock<IGRepository<Rabbit>>();

            // Opret et mock user service
            _mockUserService = new Mock<IUserService>();

            // Opret en instans af RabbitService med det mock repository og mock user service
            _rabbitService = new RabbitService(_mockRepository.Object, _mockUserService.Object, new RabbitValidator());
        }

        // Andre tests nedenfor

        [TestMethod]
        public async Task TestGetAllRabbitsByOwnerAsync()
        {
            // Arrange
            int userId = 2; // Udskift dette med den faktiske bruger-ID
            var expectedRabbitsCount = 14; // Antal kaniner forventet for den pågældende bruger

            // Konfigurer mock repository. Returner en liste af kaniner
            _mockRepository.Setup(repo => repo.GetAllObjectsAsync()).ReturnsAsync(MockRabbits.GetMockRabbits());

            // Konfigurer mock user service. Returner en bruger
            var mockUsers = MockUsers.GetMockUsers();
            _mockUserService.Setup(service => service.GetUserByIdAsync(It.IsAny<int>()))
                            .Returns<int>(id => Task.FromResult(mockUsers.FirstOrDefault(user => user.Id == id)));

            var rabbits = await _rabbitService.GetAllRabbitsByOwnerAsync(userId);

            Assert.AreEqual(expectedRabbitsCount, rabbits.Count);
        }

        [TestMethod]
        public async Task TestAddRabbitAsync()
        {
            // Arrange
            var mockRepository = new Mock<IGRepository<Rabbit>>();
            var mockValidator = new Mock<RabbitValidator>();

            // Hent nogle eksisterende brugere fra MockUsers
            var mockUsers = MockUsers.GetMockUsers();
            var user1 = mockUsers[0];
            var user2 = mockUsers[1];

            // Hent nogle eksisterende kaniner fra MockRabbits
            var mockRabbits = MockRabbits.GetMockRabbits();
            var existingRabbit1 = mockRabbits[0];
            var existingRabbit2 = mockRabbits[1];

            // Opret en ny kanin
            var newRabbit = new Rabbit { RightEarId = "1111", LeftEarId = "2222", Owner = user1.BreederRegNo };


            // Konfigurer mock repository til at returnere null for eksisterende kanin
            mockRepository.Setup(repo => repo.GetObjectByIdAsync(It.IsAny<int>())).ReturnsAsync((Rabbit)null);
            // Konfigurer mock repository til at returnere eksisterende kaniner
            mockRepository.Setup(repo => repo.GetAllObjectsAsync()).ReturnsAsync(mockRabbits);

            // Opret en instans af RabbitService med mock repository og mock validator
            var rabbitService = new RabbitService(mockRepository.Object, _mockUserService.Object, mockValidator.Object);

            // Act
            await rabbitService.AddRabbitAsync(newRabbit, user1);

            // Assert
            // Forvent, at AddObjectAsync kaldes på mock repository
            mockRepository.Verify(repo => repo.AddObjectAsync(It.IsAny<Rabbit>()), Times.Once);

            // Hent alle kaniner fra mock repository efter tilføjelsen
            var rabbits = await rabbitService.GetAllRabbitsAsync();

            // Forvent, at listen af kaniner er blevet forøget med 1
            Assert.AreEqual(mockRabbits.Count + 1, rabbits.Count);
        }

    }
}
