using DB_AngoraLib.DTOs;
using DB_AngoraLib.EF_DbContext;
using DB_AngoraLib.Models;
using DB_AngoraLib.Repository;
using DB_AngoraLib.Services.AccountService;
using DB_AngoraLib.Services.BreederBrandService;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DB_AngoraMST.Services_InMemTest
{
    [TestClass]
    public class BreederBrandServices_MST
    {
        private IBreederBrandService _breederBrandService;
        private Mock<IAccountService> _accountServiceMock;
        private DB_AngoraContext _context;

        public BreederBrandServices_MST()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<DB_AngoraContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new DB_AngoraContext(options);
            _context.Database.EnsureCreated();

            // Create mock for IAccountService
            _accountServiceMock = new Mock<IAccountService>();

            // Setup Moq for IAccountService
            _accountServiceMock.Setup(service => service.Get_UserById(It.IsAny<string>()))
                .ReturnsAsync((string userId) => _context.Users.OfType<Breeder>().FirstOrDefault(b => b.Id == userId));


            var breederBrandRepository = new GRepository<BreederBrand>(_context);
            _breederBrandService = new BreederBrandServices(breederBrandRepository, _accountServiceMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestMethod]
        public async Task Create_BreederBrand_ShouldCreateNewBreederBrand()
        {
            // Arrange
            var userId = "testUserId";
            var user = new Breeder
            {
                Id = userId,
                FirstName = "John",
                LastName = "Johnsen",
                BreederRegNo = "12345",
                City = "TestCity",
                RoadNameAndNo = "TestRoad 1"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _breederBrandService.Create_BreederBrand(userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(userId, result.UserId);
            Assert.AreEqual($"{user.LastName}'s kaninavl", result.BreederBrandName);
        }

        [TestMethod]
        public async Task Get_BreederBrandById_ShouldReturnCorrectBreederBrand()
        {
            // Arrange
            var breederBrand = _context.BreederBrands.First();
            var breederBrandId = breederBrand.Id;

            // Act
            var result = await _breederBrandService.Get_BreederBrandById(breederBrandId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(breederBrandId, result.Id);
        }

        [TestMethod]
        public async Task Get_BreederBrandByUserId_ShouldReturnCorrectBreederBrand()
        {
            // Arrange
            var breederBrand = _context.BreederBrands.First();
            var userId = breederBrand.UserId;

            // Act
            var result = await _breederBrandService.Get_BreederBrandByUserId(userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(userId, result.UserId);
        }
               

        [TestMethod]
        public async Task Update_BreederBrand_ShouldUpdateBreederBrand()
        {
            // Arrange
            var breederBrand = _context.BreederBrands.First();
            breederBrand.BreederBrandName = "Updated Name";

            // Act
            await _breederBrandService.Update_BreederBrand(breederBrand);
            var updatedBreederBrand = await _breederBrandService.Get_BreederBrandById(breederBrand.Id);

            // Assert
            Assert.IsNotNull(updatedBreederBrand);
            Assert.AreEqual("Updated Name", updatedBreederBrand.BreederBrandName);
        }

        [TestMethod]
        public async Task Delete_BreederBrand_ShouldDeleteBreederBrand()
        {
            // Arrange
            var breederBrand = _context.BreederBrands.First();
            var breederBrandId = breederBrand.Id;

            // Act
            await _breederBrandService.Delete_BreederBrand(breederBrandId);
            var deletedBreederBrand = await _breederBrandService.Get_BreederBrandById(breederBrandId);

            // Assert
            Assert.IsNull(deletedBreederBrand);
        }
    }
}
