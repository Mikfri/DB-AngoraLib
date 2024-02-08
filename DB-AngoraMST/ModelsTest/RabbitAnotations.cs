using DB_AngoraLib.Models;
using System.ComponentModel.DataAnnotations;

namespace DB_AngoraMST.ModelsTest
{
    [TestClass]
    public class RabbitAnotations
    {
        private Rabbit validAngoraRabbit = new() { Id = 1, RightEarId = "5053", LeftEarId = "001", Race = Race.Angora };
        private Rabbit notApprovedRaceColorAngora = new() { Id = 2, Race = Race.Angora, Color = Color.Chinchilla };

        //[TestMethod]
        //public void LeftEarId_ShouldPassValidation()
        //{
        //    // Arrange
        //    var validationContext = new ValidationContext(validAngoraRabbit) { MemberName = nameof(Rabbit.LeftEarId) };
        //    var validationResults = new List<ValidationResult>();

        //    // Act
        //    bool isValid = Validator.TryValidateProperty(validAngoraRabbit.LeftEarId, validationContext, validationResults);

        //    // Assert
        //    Assert.IsTrue(isValid, "Expected LeftEarId to pass validation");
        //    Assert.AreEqual(0, validationResults.Count, "Expected no validation errors"); 
        //}

        [TestMethod]
        [DataRow("123")] //3 tegn GYLDIG
        public void TestValidLeftEarId(string lefttEarId)
        {
            validAngoraRabbit.LeftEarId = lefttEarId;
            validAngoraRabbit.ValidateLeftEarId();
        }

        //[TestMethod]
        //public void LeftEarId_ShouldFailValidation()
        //{
        //    // Arrange
        //    var validationContext = new ValidationContext(notApprovedRaceColorAngora) { MemberName = nameof(Rabbit.LeftEarId) };
        //    var validationResults = new List<ValidationResult>();

        //    // Act
        //    bool isValid = Validator.TryValidateProperty(notApprovedRaceColorAngora.LeftEarId, validationContext, validationResults);

        //    // Assert
        //    Assert.IsFalse(isValid, "Expected LeftEarId to fail validation.");
        //    Assert.AreEqual(1, validationResults.Count, "Expected one validation error.");
        //    Assert.AreEqual("Skal bestå af 3 til 4 tal.", validationResults[0].ErrorMessage, "Expected specific validation error message.");
        //}
    }
}