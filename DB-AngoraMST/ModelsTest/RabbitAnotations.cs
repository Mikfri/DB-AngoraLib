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
        public void LeftEarId_ValidTest(string leftEarId)
        {
            Assert.IsTrue(validatorService.ValidateLeftEarId(leftEarId));
        }

        [TestMethod]
        [DataRow("12")]     //2 tal UGYLDIG
        [DataRow("12345")]  //5 tal UGYLDIG
        [DataRow("abc")]    //3 tegn UGYLDIG
        [DataRow("abcd")]   //4 tegn UGYLDIG
        public void LeftEarId_InvalidTest(string leftEarId)
        {
            Assert.ThrowsException<ArgumentException>(() => validatorService.ValidateLeftEarId(leftEarId));
        }

        //////////////// Race
        [TestMethod]
        [DataRow("Angora")]    // Race GYLDIG
        [DataRow("Satin")]     // Race GYLDIG
        [DataRow("saTin")]     // Race GYLDIG
        public void Race_ValidTest(string race)
        {
            Assert.IsTrue(validatorService.ValidateRace(race));
        }

        [TestMethod]
        [DataRow("Angorra")]   // Race UGYLDIG
        [DataRow("Rexx")]      // Race UGYLDIG
        public void Race_InvalidTest(string race)
        {
            Assert.ThrowsException<ArgumentException>(() => validatorService.ValidateRace(race));
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