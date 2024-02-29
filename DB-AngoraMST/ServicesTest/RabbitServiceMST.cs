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
            _rabbitService = new RabbitService(repo, vs);                     
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

        //[TestMethod()]
        //public void AddTest()
        //{
        //    Rabbit m = new() { Title = "Test", ReleaseYear = 2021 };
        //    Assert.AreEqual(6, _rabbitService.Add(m).Id);
        //    Assert.AreEqual(6, _rabbitService.GetAll().Count());

        //    Assert.ThrowsException<ArgumentOutOfRangeException>(() => _rabbitService.Add(_badRabbit));
        //}

        //[TestMethod()]
        //public void RemoveTest()
        //{
        //    Assert.IsNull(_rabbitService.Remove(100));
        //    Assert.AreEqual(1, _rabbitService.Remove(1)?.Id);
        //    Assert.AreEqual(4, _rabbitService.GetAll().Count());
        //}

        //[TestMethod()]
        //public void UpdateTest()
        //{
        //    Assert.AreEqual(5, _rabbitService.GetAll().Count());
        //    Rabbit m = new() { Title = "Test", ReleaseYear = 2024 };
        //    Assert.IsNull(_rabbitService.Update(100, m));
        //    Assert.AreEqual(1, _rabbitService.Update(1, m)?.Id); // Efter at have upd obj1, har obj1 da fortsat id 1? Ja!
        //    Assert.AreEqual(5, _rabbitService.GetAll().Count());

        //    Assert.ThrowsException<ArgumentOutOfRangeException>(() => _rabbitService.Update(1, _badRabbit));
        //}

    }
}
