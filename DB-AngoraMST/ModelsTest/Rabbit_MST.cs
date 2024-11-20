using DB_AngoraLib.Models;
using DB_AngoraLib.Services.ValidationService;
using System.ComponentModel.DataAnnotations;

namespace DB_AngoraMST.ModelsTest
{
    [TestClass]
    public class Rabbit_MST
    {
        private Rabbit rabbit = new() { RightEarId = "5053", LeftEarId = "001", Race = Race.Angora, Color = Color.Blå, };

        // LEFT EAR ID
        [TestMethod]
        [DataRow("123")]     // 3 tal GYLDIG
        [DataRow("1234")]    // 4 tal GYLDIG
        [DataRow("123456")]  // 6 tal GYLDIG
        public void LeftEarId_ValidTest(string leftEarId)
        {
            rabbit.LeftEarId = leftEarId;
            rabbit.ValidateLeftEarId();
        }

        [TestMethod]
        [DataRow(null)]   // NULL UGYLDIG
        public void LeftEarId_NullTest(string leftEarId)
        {
            rabbit.LeftEarId = leftEarId;
            Assert.ThrowsException<ArgumentNullException>(() =>
            rabbit.ValidateLeftEarId());
        }

        [TestMethod]
        [DataRow("12")]     //2 tal UGYLDIG
        [DataRow("abc")]    //3 tegn UGYLDIG
        [DataRow("abcdef")] //6 tegn UGYLDIG
        [DataRow("1234567")]//7 tal UGYLDIG
        public void LeftEarId_InvalidTest(string leftEarId)
        {
            rabbit.LeftEarId = leftEarId;
            Assert.ThrowsException<ArgumentException>(() =>
            rabbit.ValidateLeftEarId());
        }

        // RIGHT EAR ID
        [TestMethod]
        [DataRow("1234")]   // 4 tal GYLDIG
        public void RightEarId_ValidTest(string rightEarId)
        {
            rabbit.RightEarId = rightEarId;
            rabbit.ValidateRightEarId();
        }

        [TestMethod]
        [DataRow(null)]   // NULL UGYLDIG
        public void RightEarId_NullTest(string rightEarId)
        {
            rabbit.RightEarId = rightEarId;
            Assert.ThrowsException<ArgumentNullException>(() =>
            rabbit.ValidateRightEarId());
        }

        [TestMethod]
        [DataRow("123")]    // 3 tal UGYLDIG
        [DataRow("abcd")]   // 4 tegn UGYLDIG
        [DataRow("12345")]  // 5 tal UGYLDIG
        public void RightEarId_InvalidTest(string rightEarId)
        {
            rabbit.RightEarId = rightEarId;
            Assert.ThrowsException<ArgumentException>(() =>
            rabbit.ValidateRightEarId());
        }

        // RACE
        [TestMethod]
        [DataRow("Angora")]    // Race GYLDIG
        [DataRow("Satin")]     // Race GYLDIG
        [DataRow("saTin")]     // Race GYLDIG
        public void Race_ValidTest(string race)
        {
            rabbit.Race = Enum.Parse<Race>(race, true);
            rabbit.ValidateRace();
        }

        [TestMethod]
        [DataRow(null)]     // NULL UGYLDIG
        public void Race_NullTest(string race)
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                rabbit.Race = Enum.Parse<Race>(race, true);
                rabbit.ValidateRace();
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
                rabbit.ValidateRace();
            });
        }

        // COLOR
        [TestMethod]
        [DataRow("Hvid_Albino")]   // Color GYLDIG
        [DataRow("hvId_AlbiNo")]
        [DataRow("LysGråblå_Gouwenaar")]
        public void Color_ValidTest(string color)
        {
            rabbit.Color = Enum.Parse<Color>(color, true);
            rabbit.ValidateColor();
        }

        [TestMethod]
        [DataRow("Rød")]    // Color UGYLDIG
        [DataRow("Lilla")]
        public void Color_InvalidTest(string color)
        {
            Assert.ThrowsException<ArgumentException>(() =>
            {
                rabbit.Color = Enum.Parse<Color>(color, true);
                rabbit.ValidateColor();
            });
        }

        // RACE & COLOR
        [TestMethod]
        [DataRow(Race.Angora, Color.Hvid_Albino)]           // Kombi GYLDIG
        [DataRow(Race.Satin_Angora, Color.Hvid_Blåøjet)]
        public void ColorForRace_ValidTest(Race race, Color color)
        {
            Rabbit rabbit = new Rabbit { Race = race, Color = color };
            Assert.IsTrue(rabbit.ApprovedRaceColorCombination);
        }

        [TestMethod]
        [DataRow(Race.Angora, Color.Ræverød_NewZealandRed)]    // Kombi UGYLDIG
        [DataRow(Race.Satin, Color.Hvid_Albino)]
        public void ColorForRace_InvalidTest(Race race, Color color)
        {
            Rabbit rabbit = new Rabbit { Race = race, Color = color };
            Assert.IsFalse(rabbit.ApprovedRaceColorCombination);
        }

        // GENDER
        [TestMethod]
        [DataRow("Buck")]  // Gender GYLDIG
        [DataRow("01")]    // Gender GYLDIG (svarer til "Buck")
        [DataRow("Doe")]
        [DataRow("00")]
        public void Gender_ValidTest(string gender)
        {
            rabbit.Gender = Enum.Parse<Gender>(gender, true);
            rabbit.ValidateGender();
        }

        [TestMethod]
        [DataRow("Hann")]    // Gender UGYLDIG
        [DataRow("03")]
        public void Gender_InvalidTest(string gender)
        {
            Assert.ThrowsException<ArgumentException>(() =>
            {
                rabbit.Gender = Enum.Parse<Gender>(gender, true);
                rabbit.ValidateGender();
            });
        }
    }
}