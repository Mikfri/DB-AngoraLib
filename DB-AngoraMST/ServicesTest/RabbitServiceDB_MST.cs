using DB_AngoraLib.EF_DbContext;
using DB_AngoraLib.Models;
using DB_AngoraLib.Repository;
using DB_AngoraLib.Services.RabbitService;
using DB_AngoraLib.Services.UserService;
using DB_AngoraLib.Services.ValidationService;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DB_AngoraMST.ServicesTest
{
    [TestClass]
    public class RabbitServiceDB_MST
    {
        private RabbitService rabbitService;
        private DbContextOptions<DB_AngoraContext> options;

        [TestInitialize]
        public void Initialize()
        {
            // Configure options for in-memory database
            options = new DbContextOptionsBuilder<DB_AngoraContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            // Initialize RabbitService with the in-memory database context
            using (var context = new DB_AngoraContext(options))
            {
                context.Database.EnsureCreated(); // Ensure database is created
            }

            var dbContext = new DB_AngoraContext(options);
            var userRepository = new GRepository<User>(dbContext);
            var rabbitRepository = new GRepository<Rabbit>(dbContext);
            var userService = new UserService(userRepository);
            rabbitService = new RabbitService(rabbitRepository, userService, new RabbitValidator());
        }

        [TestMethod]
        public async Task AddRabbitAsync_ShouldAddNewRabbitToDatabase()
        {
            // Arrange
            var newRabbit = new Rabbit
            {
                // Set properties for the new rabbit
            };

            // Act
            await rabbitService.AddRabbitAsync(newRabbit, user);

            // Assert
            using (var context = new DB_AngoraContext(options))
            {
                var addedRabbit = context.Rabbits.FirstOrDefault(r => r.Id == newRabbit.Id);
                Assert.IsNotNull(addedRabbit);
                // Add more assertions based on your expectations
            }
        }

        // Add more tests as needed
    }
}
