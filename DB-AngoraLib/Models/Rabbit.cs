using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DB_AngoraLib.Models
{
    
    public enum IsPublic
    {
        Yes,
        No
    }

    public enum Gender
    {
        Male,
        Female
    }

    public enum Race
    {
        Angora,
        Belgisk_Hare,
        Belgisk_Kæmpe,
        Beveren,

        Hermelin,
        Hollænder,
        Hotot,

        Lille_Chinchilla,
        Lille_Havana,
        Lille_Rex,
        Lille_Satin,

        Lux,
        Løvehoved,

        Rex,
        Sallander,
        Satin,
        Satinangora,

        Stor_Chinchilla,
        Stor_Havana,
    }

    public enum Color
    {
        // Vildtanlægfarver
        Vildtgrå,
        Jerngrå,
        Vildtsort,
        Vildtgul,
        Vildtbrun,
        Vildtblå_PerleEgern,
        Vildtrød_Harefarvet,
        Rødbrun_Gråblå_Lux,
        Gulrød_Bourgogne,
        Orange,
        Ræverød_NewZealandRed,
        Lutino,
        Lutino_Shadow,
        Chinchilla,
        Schwarzgrannen,

        // Ensfarvede
        Sort_Alaska,
        Blå,
        LyseBlå_BlåBeveren,
        LilleEgern_Gråblå,
        MarburgerEgern_Gråblå,
        Gouwenaar,
        Brun_Havana,
        Beige,
        Rødorange_Sachsengold,
        Hvid,

        // Ensfarvede m. slør
        Rødbrun_Madagascar,
        Gulbrun_Isabella,
        Sallander,

        // Ensfarvede m. stikkelhår
        Sølv,
        Stikkelhår_Trønder
    }

    public class Rabbit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string? Owner { get; set; }

        [RegularExpression(@"^\d{3,}$", ErrorMessage = "Skal bestå af mindst 3 tal.")]  // todo: check op om det er rigtigt.. Hvad med over 4 tal??
        public string LeftEarId { get; set; }

        [RegularExpression(@"^\d{4}$", ErrorMessage = "Skal bestå af 4 tal!")]
        public string RightEarId { get; set; }

        public string NickName { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public DateOnly? DateOfDeath { get; set; }

        public Gender? Gender { get; set; }

        public Race? Race { get; set; }

        public Color? Color { get; set; }

        public IsPublic? IsPublic { get; set; }


        public Rabbit(int id, string leftEarId, string rightEarId, string? owner, string nickName, DateOnly dateOfBirth, DateOnly? dateOfDeath, Gender? gender, Race? race, Color? color, IsPublic? isPublic)
        {
            Id = id;
            LeftEarId = leftEarId;
            RightEarId = rightEarId;
            Owner = owner;
            NickName = nickName;
            DateOfBirth = dateOfBirth;
            DateOfDeath = dateOfDeath;
            Gender = gender;
            Race = race;
            Color = color;
            IsPublic = isPublic;
        }
        public Rabbit() { }

        public void ValidateLeftEarId()
        {
            if (LeftEarId == null)
            {
                throw new ArgumentNullException("NULL: Kanin ID, skal udfyldes");
            }

            if (LeftEarId.Length < 1 || LeftEarId.Length < 4 )
            {
                throw new ArgumentException($"Kanin ID, skal være imellem 1-4 numrer langt");
            }
        }
    }
}
