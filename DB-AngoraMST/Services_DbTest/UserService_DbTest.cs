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


    
    }

}
