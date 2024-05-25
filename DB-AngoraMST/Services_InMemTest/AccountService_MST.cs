using DB_AngoraLib.DTOs;
using DB_AngoraLib.EF_DbContext;
using DB_AngoraLib.MockData;
using DB_AngoraLib.Models;
using DB_AngoraLib.Repository;
using DB_AngoraLib.Services.AccountService;
using DB_AngoraLib.Services.EmailService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DB_AngoraMST.Services_InMemTest
{
    [TestClass]
    public class AccountServices_MST
    {
        private IAccountService _accountService;
        private DB_AngoraContext _context;
        private Mock<UserManager<User>> _userManagerMock;
        private Mock<IEmailService> _emailServiceMock;

        public AccountServices_MST()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<DB_AngoraContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new DB_AngoraContext(options);
            _context.Database.EnsureCreated();

            // Create UserManager
            var userStoreMock = new Mock<IUserStore<User>>();
            _userManagerMock = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

            // Create EmailService
            _emailServiceMock = new Mock<IEmailService>();

            var userRepository = new GRepository<User>(_context);
            _accountService = new AccountServices(_userManagerMock.Object, _emailServiceMock.Object);
        }

        [TestInitialize]
        public void Setup()
        {
            // Add mock data to in-memory database
            var mockUsers = MockUsers.GetMockUsers();
            _context.Users.AddRange(mockUsers);
            _context.SaveChanges();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        // Add your test methods here

        //[TestMethod]
        //public async Task Register_BasicUserAsync_Test()
        //{
        //    // Arrange
        //    var newUserDto = new User_CreateBasicDTO
        //    {
        //        Email = "testuser@gmail.com",
        //        Password = "Test123!",
        //        Phone = "1234567890",
        //        FirstName = "Test",
        //        LastName = "User",
        //        RoadNameAndNo = "Test Road 1",
        //        ZipCode = 12345,
        //        City = "Test City"
        //    };

        //    _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
        //        .ReturnsAsync(IdentityResult.Success);
        //    _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
        //        .ReturnsAsync(IdentityResult.Success);
        //    _userManagerMock.Setup(x => x.AddClaimsAsync(It.IsAny<User>(), It.IsAny<IEnumerable<Claim>>()))
        //        .ReturnsAsync(IdentityResult.Success);

        //    // Ensure FindByNameAsync and GetClaimsAsync return non-null values
        //    _userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
        //        .ReturnsAsync(new User { UserName = newUserDto.Email });
        //    _userManagerMock.Setup(x => x.GetClaimsAsync(It.IsAny<User>()))
        //        .ReturnsAsync(new List<Claim> { new Claim(ClaimTypes.Name, newUserDto.Email) });

        //    // Act
        //    var result = await _accountService.Register_BasicUserAsync(newUserDto);

        //    // Assert
        //    Assert.IsNotNull(result);
        //    Assert.IsTrue(result.IsSuccessful);
        //    Assert.AreEqual(newUserDto.Email, result.UserName);

        //    // Get the created user and their claims
        //    var createdUser = await _userManagerMock.Object.FindByNameAsync(result.UserName);
        //    var userClaims = await _userManagerMock.Object.GetClaimsAsync(createdUser);

        //    // Check that the user has the expected claims
        //    var expectedClaims = new List<Claim>
        //    {
        //        new Claim(ClaimTypes.Name, newUserDto.Email),
        //        // Add other expected claims here
        //    };
        //    foreach (var expectedClaim in expectedClaims)
        //    {
        //        Assert.IsTrue(userClaims.Any(uc => uc.Type == expectedClaim.Type && uc.Value == expectedClaim.Value));
        //    }
        //}


    }
}
