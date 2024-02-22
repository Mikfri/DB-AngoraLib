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

        private Rabbit rabbit = new() { Id = 1, RightEarId = "5053", LeftEarId = "001", Race = Race.Angora, Color = Color.Blå, };


        //////////////// LeftEarId
        [TestMethod]
        [DataRow("123")]    // 3 tal GYLDIG
        [DataRow("1234")]   // 4 tal GYLDIG
        [DataRow("12345")]  // 5 tal GYLDIG
        public void LeftEarId_ValidTest(string leftEarId)
        {
            rabbit.LeftEarId = leftEarId;
            validatorService.ValidateLeftEarId(rabbit);
        }

        [TestMethod]
        [DataRow(null)]   // NULL UGYLDIG
        public void LeftEarId_NullTest(string leftEarId)
        {
            rabbit.LeftEarId = leftEarId;
            Assert.ThrowsException<ArgumentNullException>(() => validatorService.ValidateLeftEarId(rabbit));
        }

        [TestMethod]
        [DataRow("12")]     //2 tal UGYLDIG
        [DataRow("123456")] //6 tal UGYLDIG
        [DataRow("abc")]    //3 tegn UGYLDIG
        [DataRow("abcd")]   //4 tegn UGYLDIG
        public void LeftEarId_InvalidTest(string leftEarId)
        {
            rabbit.LeftEarId = leftEarId;
            Assert.ThrowsException<ArgumentException>(() => validatorService.ValidateLeftEarId(rabbit));
        }

        //////////////// RightEarId
        [TestMethod]
        [DataRow("1234")]   // 4 tal GYLDIG
        public void rightEarId_ValidTest(string rightEarId)
        {
            rabbit.RightEarId = rightEarId;
            validatorService.ValidateRightEarId(rabbit);
        }

        [TestMethod]
        [DataRow(null)]   // NULL UGYLDIG
        public void rightEarId_NullTest(string rightEarId)
        {
            rabbit.RightEarId = rightEarId;
            Assert.ThrowsException<ArgumentNullException>(() => validatorService.ValidateRightEarId(rabbit));
        }

        [TestMethod]
        [DataRow("123")]    // 3 tal UGYLDIG
        [DataRow("abcd")]   // 4 tegn UGYLDIG
        [DataRow("12345")]  // 5 tal UGYLDIG
        public void rightEarId_InvalidTest(string rightEarId)
        {
            rabbit.RightEarId = rightEarId;
            Assert.ThrowsException<ArgumentException>(() => validatorService.ValidateRightEarId(rabbit));
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
        [DataRow(null)]     // NULL UGYLDIG
        public void Race_NullTest(string race)
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                rabbit.Race = Enum.Parse<Race>(race, true);
                validatorService.ValidateRace(rabbit);
            });
        }

        [TestMethod]
        [DataRow("Angorra")]   // Race UGYLDIG
        [DataRow("Rexx")]   
        public void Race_InvalidTest(string race)
        {
            Assert.ThrowsException<ArgumentException>(() =>
            {
                rabbit.Race = Enum.Parse<Race>(race, true);
                validatorService.ValidateRace(rabbit);
            });
            //Assert.ThrowsException<ArgumentException>(() => validatorService.ValidateRace(new Rabbit { Race = Enum.Parse<Race>(race, true) })); // Alternativt
        }


        //////////////// Color
        [TestMethod]
        [DataRow("Hvid")]   // Color GYLDIG
        [DataRow("hvId")]
        [DataRow("Gouwenaar")]
        public void Color_ValidTest(string color)
        {
            rabbit.Color = Enum.Parse<Color>(color, true);
            validatorService.ValidateColor(rabbit);
        }

        [TestMethod]
        [DataRow("Rød")]    // Color UGYLDIG
        [DataRow("Lilla")]
        public void Color_InvalidTest(string color)
        {
            Assert.ThrowsException<ArgumentException>(() =>
            {
                rabbit.Color = Enum.Parse<Color>(color, true);
                validatorService.ValidateColor(rabbit);
            });
        }

        /////////////// Race && Color
        [TestMethod]
        [DataRow(Race.Angora, Color.Hvid)]           // Kombi GYLDIG
        [DataRow(Race.Satinangora, Color.Hvid)]
        public void ColorForRace_ValidTest(Race race, Color color)
        {
            Rabbit rabbit = new Rabbit { Race = race, Color = color };
            bool validRaceColorCombo = validatorService.ValidateRaceAndColorCombo(rabbit);
            Assert.IsTrue(validRaceColorCombo);
        }

        [TestMethod]
        [DataRow(Race.Angora, Color.Chinchilla)]    // Kombi UGYLDIG
        [DataRow(Race.Satin, Color.Hvid)]
        public void ColorForRace_InvalidTest(Race race, Color color)
        {
            Rabbit rabbit = new Rabbit { Race = race, Color = color };

            bool validRaceColorCombo = validatorService.ValidateRaceAndColorCombo(rabbit);

            Assert.IsFalse(validRaceColorCombo);
        }

        //////////////// Gender
        [TestMethod]
        [DataRow("Han")]   // Gender GYLDIG
        [DataRow("01")]   // Gender GYLDIG
        [DataRow("Hun")]
        [DataRow("00")]
        public void Gender_ValidTest(string gender)
        {
            rabbit.Gender = Enum.Parse<Gender>(gender, true);
            validatorService.ValidateGender(rabbit);
        }

        [TestMethod]
        [DataRow("Hann")]    // Gender UGYLDIG
        [DataRow("03")]
        public void Gender_InvalidTest(string gender)
        {
            Assert.ThrowsException<ArgumentException>(() =>
            {
                rabbit.Gender = Enum.Parse<Gender>(gender, true);
                validatorService.ValidateGender(rabbit);
            });
        }
    }
}