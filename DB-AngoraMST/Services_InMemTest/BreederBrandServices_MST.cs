using DB_AngoraLib.EF_DbContext;
using DB_AngoraLib.MockData;
using DB_AngoraLib.Models;
using DB_AngoraLib.Repository;
using DB_AngoraLib.Services.AccountService;
using DB_AngoraLib.Services.BreederBrandService;
using DB_AngoraLib.Services.EmailService;
using DB_AngoraLib.Services.RabbitService;
using DB_AngoraLib.Services.ValidationService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraMST.Services_InMemTest
{
    [TestClass]
    public class BreederBrandServices_MST
    {
        //private IBreederBrandService _bbService;
        //private IAccountService _accountService;
        //private DB_AngoraContext _context;
        //private Mock<UserManager<User>> _userManagerMock;
        //private Mock<IEmailService> _emailServiceMock;


        //public BreederBrandServices_MST()
        //{
        //    // Setup in-memory database
        //    var options = new DbContextOptionsBuilder<DB_AngoraContext>()
        //        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        //        .Options;

        //    _context = new DB_AngoraContext(options);
        //    _context.Database.EnsureCreated();

        //    // Create UserManager
        //    var userStoreMock = new Mock<IUserStore<User>>();
        //    _userManagerMock = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

        //    // Create EmailService mock
        //    _emailServiceMock = new Mock<IEmailService>();

        //    var userRepository = new GRepository<User>(_context);
        //    _accountService = new AccountServices(userRepository, _emailServiceMock.Object, _userManagerMock.Object);

        //    var bbRepository = new GRepository<BreederBrand>(_context);
        //    _bbService = new BreederBrandServices(bbRepository, _accountService);
        //}

        //[TestInitialize]
        //public void Setup()
        //{
        //    // Add mock data to in-memory database
        //    var mockDataInitializer = new MockDataInitializer(_context, _userManagerMock.Object);
        //    mockDataInitializer.Initialize();

        //    _context.SaveChanges();
        //}


        //[TestCleanup]
        //public void Cleanup()
        //{
        //    _context.Database.EnsureDeleted();
        //    _context.Dispose();
        //}

        //[TestMethod]
        //public async Task Get_BreederBrandByBreederRegNo_ShouldReturnCorrectBreederBrand()
        //{
        //    // Arrange
        //    var idasId = "IdasId";
        //    var idasBreederBrand = _context.BreederBrands.First(bb => bb.UserId == idasId);
        //    //var otherBreederBrand = _context.BreederBrands.First(bb => bb.UserId != idasId);

        //    Console.WriteLine($"BreederBrand.Id: {idasBreederBrand.Id}\nBreederBrand.UserId: {idasBreederBrand.UserId}\nUser.Name: {idasBreederBrand.User.FirstName}"); 

        //    // Act
        //    //var result = await _bbService.Get_BreederBrandByUserId(idasId);
        //    var result = await _bbService.Get_BreederBrandById(1);
        //    //var result = await _bbService.Get_BreederBrandById(1);

        //    // Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(idasBreederBrand.UserId, result.UserId);
        //    Assert.AreEqual(idasBreederBrand.BreederBrandName, result.BreederBrandName);
        //}
    }
}
