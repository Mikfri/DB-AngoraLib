using DB_AngoraLib.Models;
using DB_AngoraLib.Services.ValidationService;
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
        [DataRow("123")]    // 3 tal GYLDIG
        [DataRow("1234")]   // 4 tal GYLDIG
        public void LeftEarId_ValidTest(string leftEarId)
        {
            rabbit.LeftEarId = leftEarId;
            validatorService.ValidateLeftEarId(rabbit);
        }

        [TestMethod]
        [DataRow("12")]     //2 tal UGYLDIG
        [DataRow("12345")]  //5 tal UGYLDIG
        [DataRow("abc")]    //3 tegn UGYLDIG
        [DataRow("abcd")]   //4 tegn UGYLDIG
        public void LeftEarId_InvalidTest(string leftEarId)
        {
            rabbit.LeftEarId = leftEarId;
            Assert.ThrowsException<ArgumentException>(() => validatorService.ValidateLeftEarId(rabbit));
        }

        //////////////// Race
        [TestMethod]
        [DataRow("Angora")]    // Race GYLDIG
        [DataRow("Satin")]     // Race GYLDIG
        [DataRow("saTin")]     // Race GYLDIG
        public void Race_ValidTest(string race)
        {
            rabbit.Race = Enum.Parse<Race>(race, true);
            validatorService.ValidateRace(rabbit);
        }

        [TestMethod]
        [DataRow("Angorra")]   // Race UGYLDIG
        [DataRow("Rexx")]      // Race UGYLDIG
        public void Race_InvalidTest(string race)
        {
            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() => validatorService.ValidateRace(new Rabbit { Race = Enum.Parse<Race>(race, true) }));
        }

        //////////////// Race && Color
        [TestMethod]
        [DataRow(Race.Angora, Color.Hvid)]           // Kombi GYLDIG
        [DataRow(Race.Satinangora, Color.Hvid)]       // Kombi GYLDIG
        public void ColorForRace_ValidTest(Race race, Color color)
        {
            bool isColorValid = validatorService.IsColorValidForRace(race, color);

            Assert.IsTrue(isColorValid);
        }

        [TestMethod]
        [DataRow(Race.Angora, Color.Chinchilla)]    // Kombi UGYLDIG
        [DataRow(Race.Satin, Color.Hvid)]           // Kombi UGYLDIG
        public void ColorForRace_InvalidTest(Race race, Color color)
        {
            // Act
            bool isColorValid = validatorService.IsColorValidForRace(race, color);

            // Assert
            Assert.IsFalse(isColorValid);
        }

    }
}