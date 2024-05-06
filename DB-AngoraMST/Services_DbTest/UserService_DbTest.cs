using DB_AngoraLib.DTOs;
using DB_AngoraLib.EF_DbContext;
using DB_AngoraLib.Models;
using DB_AngoraLib.Repository;
using DB_AngoraLib.Services.UserService;
using Microsoft.EntityFrameworkCore;


namespace DB_AngoraMST.Services_DbTest
{
    [TestClass]
    public class UserService_DBTest
    {
        private UserService _userService;
        private DB_AngoraContext _context;

        [TestInitialize]
        public void Setup()
        {
            // Setup actual database
            var options = new DbContextOptionsBuilder<DB_AngoraContext>()
                .UseSqlServer(@"Data Source=(localdb)\mssqllocaldb;Database=DB-Angora_DB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true") // Replace with your actual connection string
                .Options;

            _context = new DB_AngoraContext(options);

            var userRepository = new GRepository<User>(_context);
            _userService = new UserService(userRepository);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        [TestMethod]
        public async Task GetCurrentUsersRabbitCollection_WithoutProperties_DBTest()
        {
            // Arrange
            var currentUser = _context.Users.First();
            var userKeyDto = new User_KeyDTO { BreederRegNo = currentUser.Id };

            // Act
            var result = await _userService.GetCurrentUsersRabbitCollection_ByProperties(userKeyDto, null, null, null, null, null, null, null, null, null, null);

            // Assert
            Assert.IsNotNull(result);
            // Assert other conditions based on your actual database data
        }
    }

}
