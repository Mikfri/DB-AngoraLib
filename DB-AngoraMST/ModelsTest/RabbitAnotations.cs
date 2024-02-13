using DB_AngoraLib.Models;
using DB_AngoraLib.Services;
using System.ComponentModel.DataAnnotations;

namespace DB_AngoraMST.ModelsTest
{
    [TestClass]
    public class RabbitAnotations
    {
        // Arrange
        private ValidatorService validatorService = new ValidatorService(); 

        private Rabbit rabbit = new() { Id = 1, RightEarId = "5053", LeftEarId = "001", Race = Race.Angora };


        //////////////// LeftEarId
        [TestMethod]
        [DataRow("123")]    //3 tal GYLDIG
        [DataRow("1234")]   //4 tal GYLDIG
        public void ValidLeftEarIdTest(string lefttEarId)
        {
            rabbit.LeftEarId = lefttEarId;
            rabbit.ValidateLeftEarId();
        }

        [TestMethod]
        [DataRow("12")]     //2 tal UGYLDIG
        [DataRow("12345")]  //5 tal UGYLDIG
        [DataRow("abc")]    //3 tegn UGYLDIG
        [DataRow("abcd")]   //4 tegn UGYLDIG
        public void UnValidLeftEarIdTest(string lefttEarId)
        {
            rabbit.LeftEarId = lefttEarId;
            Assert.ThrowsException<ArgumentException>(rabbit.ValidateLeftEarId);
        }

        //////////////// Race
        [TestMethod]
        [DataRow("Angora")]    // Race GYLDIG
        [DataRow("Satin")]     // Race GYLDIG
        [DataRow("saTin")]     // Race GYLDIG
        public void ValidRaceTest(string race)
        {
            // Arrange
            var rabbit = new Rabbit();

            // Act & Assert
            Assert.IsTrue(rabbit.ValidateRace(race), $"Forventede {race} som gyldig race");
        }

        [TestMethod]
        [DataRow("Angorra")]   // Race UGYLDIG
        [DataRow("Rexx")]      // Race UGYLDIG
        public void UnValidRaceTest(string race)
        {
            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() => rabbit.ValidateRace(race));
        }

        [TestMethod]
        [DataRow(Race.Angora, Color.Hvid)]            // Gyldig kombination
        [DataRow(Race.Satinangora, Color.Hvid)]       // Gyldig kombination
        public void ValidColorForRaceTest(Race race, Color color)
        {
            bool isColorValid = validatorService.IsColorValidForRace(race, color);

            Assert.IsTrue(isColorValid);
        }

        [TestMethod]
        [DataRow(Race.Angora, Color.Chinchilla)]      // Ugyldig kombination
        [DataRow(Race.Satin, Color.Hvid)]           // Ugyldig kombination
        public void UnValidColorForRaceTest(Race race, Color color)
        {
            // Act
            bool isColorValid = validatorService.IsColorValidForRace(race, color);

            // Assert
            Assert.IsFalse(isColorValid);
        }

    }
}