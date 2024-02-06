using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DB_AngoraLib.Models
{
    public enum IsPublic
    {
        Ja,
        Nej
    }

    public enum Sex
    {
        Han,
        Hun
    }

    public class Rabbit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? Owner { get; set; }

        [RegularExpression(@"^\d{3,}$", ErrorMessage = "Skal bestå af mindst 3 tal.")]  // todo: check op om det er rigtigt.. Hvad med over 4 tal??
        public int LeftEarId { get; set; }

        [RegularExpression(@"^\d{4}$", ErrorMessage = "Skal bestå af 4 tal!")]
        public string RightEarId { get; set; }

        public string Name { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public DateOnly? DateOfDeath { get; set; }

        public Sex? Sex { get; set; }

        public IsPublic? IsPublic { get; set; }


        public Rabbit(int id, int leftEarId, int rightEarId, int? owner, string name, DateOnly dateOfBirth, DateOnly? dateOfDeath, Sex? sex, IsPublic? isPublic)
        {
            Id = id;
            LeftEarId = leftEarId;
            RightEarId = rightEarId;
            Owner = owner;
            Name = name;
            DateOfBirth = dateOfBirth;
            DateOfDeath = dateOfDeath;
            Sex = sex;
            IsPublic = isPublic;
        }
        public Rabbit() { }

        public void ValidateLeftEarId()
        {
            if (LeftEarId == null)
            {
                throw new ArgumentNullException("NULL: Kanin ID, skal udfyldes");
            }

            if (LeftEarId < 1 || LeftEarId > 9999)
            {
                throw new ArgumentException($"Kanin ID, skal være imellem 1-4 numrer langt");
            }
        }
    }
}
