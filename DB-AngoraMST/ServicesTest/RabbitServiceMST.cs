using DB_AngoraLib.Models;
using DB_AngoraLib.Services.RabbitService;
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
        public RabbitService _rabbitService;

        [TestInitialize]
        public void Initialize()
        {

        }

        private readonly Rabbit _badRabbit = new() { NickName = "Pippi", };


        [TestMethod()]
        public async Task GetAllRabbitTest()
        {
            IEnumerable<Rabbit> allRabbits = await _rabbitService.GetAllRabbitsAsync();
            Assert.AreEqual(3, allRabbits.Count());
        }

        [TestMethod()]
        public void GetByIdTest()
        {
            Assert.IsNull(_rabbitService.GetRabbitByIdAsync(-3));
        }
           
    }
}
