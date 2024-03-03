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
    public class RabbitServiceEFDB_MST
    {
        private static RabbitService rabbitService;
        private static DbContextOptions<DB_AngoraContext> options;
        private static UserService userService;

        private static void ConfigureEFDatabase()
        {
            options = new DbContextOptionsBuilder<DB_AngoraContext>()
                .UseSqlServer(DBSecrets.ConnectionStringSimply)
                .Options;

            using (var context = new DB_AngoraContext(options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }

            var dbContext = new DB_AngoraContext(options);
            var userRepository = new GRepository<User>(options); // Brug options her
            var rabbitRepository = new GRepository<Rabbit>(options); // Brug options her
            userService = new UserService(userRepository);
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

            // Set the current user for the test
            var currentUser = new User { /* user properties */ };
            UserService.SetCurrentUser(currentUser);

            // Act
            await rabbitService.AddRabbitAsync(newRabbit, UserService.GetCurrentUser());

            // Assert
            using (var context = new DB_AngoraContext(options))
            {
                var addedRabbit = context.Rabbits.FirstOrDefault(r => r.Id == newRabbit.Id);
                Assert.IsNotNull(addedRabbit);
                // Add more assertions based on your expectations
            }

            // Clear the current user after the test
            UserService.ClearCurrentUser();
        }

        // Add more tests as needed
    }
}
