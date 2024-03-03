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
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraMST.ServicesTest
{
    [TestClass]
    public class RabbitServiceMoq_MST
    {
        private static Mock<IGRepository<Rabbit>> mockRepository;
        private static Mock<IUserService> mockUserService;
        private static Mock<RabbitValidator> mockValidator;
        private static RabbitService rabbitService;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            mockRepository = new Mock<IGRepository<Rabbit>>();
            mockUserService = new Mock<IUserService>();
            mockValidator = new Mock<RabbitValidator>();
            rabbitService = new RabbitService(mockRepository.Object, mockUserService.Object, mockValidator.Object);

            // Mock the common GetObjectAsync setup
            mockRepository
                .Setup(r => r.GetObjectAsync(It.IsAny<Expression<Func<Rabbit, bool>>>()))
                .ReturnsAsync((Expression<Func<Rabbit, bool>> filter) =>
                    MockRabbits.GetMockRabbits().AsQueryable().FirstOrDefault(filter.Compile()));
        }

        //------------------------- GET METHODS TESTS -------------------------
        //[TestMethod]
        //public async Task GetAllRabbitsAsync_ShouldReturnListOfRabbits()
        //{
        //    // Arrange
        //    var expectedRabbits = MockRabbits.GetMockRabbits();
        //    mockRepository.Setup(r => r.GetAllObjectsAsync()).ReturnsAsync(expectedRabbits);

        //    // Act
        //    var result = await rabbitService.GetAllRabbitsAsync();

        //    // Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(expectedRabbits.Count, result.Count);
        //}

        //[TestMethod]
        //public async Task GetAllRabbitsByOwnerAsync_ShouldReturnRabbitsOwnedByUser()
        //{
        //    // Arrange
        //    var user = MockUsers.GetMockUsers().First();
        //    var userRabbits = MockRabbits.GetMockRabbits().Where(r => r.Owner == user.BreederRegNo).ToList();

        //    mockUserService.Setup(u => u.GetUserByIdAsync(user.Id)).ReturnsAsync(user);
        //    mockRepository.Setup(r => r.GetAllObjectsAsync()).ReturnsAsync(MockRabbits.GetMockRabbits());

        //    // Act
        //    var userRabbitsResult = await rabbitService.GetAllRabbitsByOwnerAsync(user.Id);

        //    // Assert
        //    Assert.IsNotNull(userRabbitsResult);
        //    Assert.AreEqual(userRabbits.Count, userRabbitsResult.Count);
        //}


        //[TestMethod]
        //public async Task GetRabbitByEarTagsAsync_Found_TEST()
        //{
        //    // Arrange
        //    var existingRightEarId = "5095";
        //    var existingLeftEarId = "003";

        //    //Act
        //    var result = await rabbitService.GetRabbitByEarTagsAsync(existingRightEarId, existingLeftEarId);

        //    // Assert
        //    Assert.IsNotNull(result);
        //}


        //[TestMethod]
        //public async Task GetRabbitByEarTagsAsync_NoneFound_TEST()
        //{
        //    // Arrange
        //    var nonExistRightEarId = "5095";
        //    var nonExistLeftEarId = "103";

        //   //Act
        //   var result = await rabbitService.GetRabbitByEarTagsAsync(nonExistRightEarId, nonExistLeftEarId);

        //    // Assert
        //    Assert.IsNull(result);
        //}
                

        ////------------------------- ADD METHODS TESTS -------------------------
        //[TestMethod]    // todo: virker ikke helt korrekt..
        //public async Task AddRabbitAsyncTEST()
        //{
        //    // Arrange
        //    var newRabbit = new Rabbit
        //    {
        //        Id = 1,
        //        RightEarId = "5095",
        //        LeftEarId = "103",
        //        Owner = "5095",
        //        NickName = "TestWab",
        //        Race = Race.Angora,
        //        Color = Color.LilleEgern_Gråblå,
        //        DateOfBirth = new DateOnly(2021, 01, 01),
        //        Gender = Gender.Hun,
        //    };
        //    var user = MockUsers.GetMockUsers().First(); // Get the first user for testing

        //    // Mock the AddObjectAsync method to return a Task.FromResult(0) (indicating successful addition)
        //    mockRepository.Setup(r => r.AddObjectAsync(newRabbit)).Returns(Task.FromResult(0));

        //    // Mock the GetAllRabbitsAsync method to return a list of rabbits after addition
        //    var rabbitsBeforeAddition = MockRabbits.GetMockRabbits();
        //    var rabbitsAfterAddition = new List<Rabbit>(rabbitsBeforeAddition) { newRabbit };
        //    mockRepository.Setup(r => r.GetAllObjectsAsync()).ReturnsAsync(rabbitsAfterAddition);

        //    // Act
        //    await rabbitService.AddRabbitAsync(newRabbit, user);

        //    // Assert
        //    // Verify that the count of rabbits increased by 1
        //    var rabbitsBeforeCount = rabbitsBeforeAddition.Count;
        //    var rabbitsAfterCount = rabbitsAfterAddition.Count;
        //    Assert.AreEqual(rabbitsBeforeCount + 1, rabbitsAfterCount);
        //}


    }
}
