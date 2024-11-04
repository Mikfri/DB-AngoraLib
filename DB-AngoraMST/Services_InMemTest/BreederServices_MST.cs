using DB_AngoraLib.DTOs;
using DB_AngoraLib.EF_DbContext;
using DB_AngoraLib.MockData;
using DB_AngoraLib.Models;
using DB_AngoraLib.Repository;
using DB_AngoraLib.Services.BreederService;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DB_AngoraMST.Services_InMemTest
{
    [TestClass]
    public class BreederServices_MST
    {
        private IBreederService _breederService;
        private DB_AngoraContext _context;

        public BreederServices_MST()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<DB_AngoraContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new DB_AngoraContext(options);
            _context.Database.EnsureCreated();

            // Create BreederRepository
            var breederRepository = new GRepository<Breeder>(_context);

            // Initialize BreederService with all required parameters
            _breederService = new BreederServices(breederRepository);
        }

        [TestInitialize]
        public void Setup()
        {
            // Add mock data to in-memory database
            var mockUsersWithRoles = MockUsers.GetMockUsersWithRoles();
            foreach (var mockUserWithRole in mockUsersWithRoles)
            {
                _context.Users.Add(mockUserWithRole.User);
                _context.SaveChanges();
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        //---------------------------------: GET TESTS :---------------------------------
        [TestMethod]
        public async Task GetAllBreedersAsync_TEST()
        {
            // Arrange
            var expectedBreedersCount = _context.Users.OfType<Breeder>().Count();

            // Act
            var breeders = await _breederService.GetAll_Breeders();

            foreach (var breeder in breeders)
            {
                Console.WriteLine($"Breeder: {breeder.FirstName} {breeder.LastName}");
            }

            // Assert
            Assert.AreEqual(expectedBreedersCount, breeders.Count);
        }

        [TestMethod]
        public async Task GetBreederByBreederRegNoAsync_TEST()
        {
            // Arrange
            var expectedUser = _context.Users.OfType<Breeder>().First();
            var breederRegNo = expectedUser.BreederRegNo;

            // Act
            var actualUser = await _breederService.Get_BreederByBreederRegNo(breederRegNo);

            // Assert
            Assert.IsNotNull(actualUser);
            Assert.AreEqual(expectedUser.BreederRegNo, actualUser.BreederRegNo);
        }

        [TestMethod]
        public async Task GetRabbitsOwnedFiltered_TEST()
        {
            // Arrange
            var userId = "MajasId";
            var race = Race.Satin_Angora;
            var dateOfBirth = new DateOnly(2024, 3, 1);

            var filter = new Rabbit_FilteredRequestDTO
            {
                Race = race,
                FromDateOfBirth = dateOfBirth,
                OnlyDeceased = false
            };

            // Get the user's rabbit collection for comparison
            var mockUser = _context.Users
                .Include(u => ((Breeder)u).RabbitsOwned) // Load the related rabbits
                .Single(u => u.Id == userId) as Breeder; // Get the user and cast to Breeder

            var mockUserRabbitCollection = mockUser.RabbitsOwned;

             // Filtrer mockUserRabbitCollection baseret på DateOfBirth
            var filteredMockUserRabbitCollection = mockUserRabbitCollection
                .Where(rabbit => rabbit.DateOfBirth >= filter.FromDateOfBirth)
                .ToList();


            Console.WriteLine($"Breeder: {mockUser.FirstName}\nICollection: RabbitsOwned\n");
            // Print hver filtreret kanins kaldenavn og fødselsdato for debugging formål
            int i = 1;
            foreach (var rabbit in filteredMockUserRabbitCollection)
            {
                Console.WriteLine($"{i++}:{rabbit.EarCombId}: {rabbit.NickName}, DateOfBirth: {rabbit.DateOfBirth}");
            }

            // Act
            var filteredRabbitCollection = await _breederService.GetAll_RabbitsOwned_Filtered(userId, filter);

            // Assert
            Assert.IsNotNull(filteredRabbitCollection); // Check that the result is not null
            Assert.IsTrue(filteredRabbitCollection.All(rabbit => rabbit.Race == race)); // Check that all rabbits in the result have the expected race

            // Check that the number of rabbits in the result matches the expected number
            var allRabbits = await _context.Rabbits.ToListAsync();
            var expectedRabbitCount = allRabbits.Count(rabbit => rabbit.OwnerId == userId && rabbit.Race == race && rabbit.DateOfBirth >= filter.FromDateOfBirth);

            Assert.AreEqual(expectedRabbitCount, filteredRabbitCollection.Count);
            Console.WriteLine($"\nExpected: {expectedRabbitCount}, Actual: {filteredRabbitCollection.Count}");
        }
    }
}