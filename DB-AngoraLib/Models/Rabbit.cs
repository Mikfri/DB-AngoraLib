using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DB_AngoraLib.Models
{    
    public enum IsPublic
    {
        No,
        Yes        
    }

    public enum Gender
    {
        Han = 1,
        Hun,        //nu har 'Hun' automatisk value = 2
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
        Jamora,

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

    public enum Pattern
    {
        Hotot,
        TyskSchecke_TKS_LTS,
        EngelskSchecke,
        RhinskSchecke,
        Dalmatiner,
        Kappe,
        Japaner,
        Hollænder,
        Tan_White_Otter,
        Russer,
        Zobel_Siameser,
    }

    public class Rabbit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        [ForeignKey("User")]
        public string? Owner { get; set; }
        public virtual User user { get; set; } // virtual -> lazy loading (færre DB requests)


        [RegularExpression(@"^\d{4}$", ErrorMessage = "Kanin.AvlerNo: Skal bestå af 4 tal!")]
        public string RightEarId { get; set; }

        [RegularExpression(@"^\d{3,5}$", ErrorMessage = "Kanin.Id: Min 3 tal. Max 5 tal")]
        public string LeftEarId { get; set; }        

        public string? NickName { get; set; }
        public Race Race { get; set; }

        public Color Color { get; set; }

        public bool? ApprovedRaceColorCombination { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public DateOnly? DateOfDeath { get; set; }

        public Gender Gender { get; set; } 
        public IsPublic? IsPublic { get; set; }


        public Rabbit(int id, string? owner, User user, string rightEarId, string leftEarId, string? nickName, Race race, Color color, bool? approvedRaceColorCombination, DateOnly dateOfBirth, DateOnly? dateOfDeath, Gender gender, IsPublic? isPublic)
        {
            Id = id;
            Owner = owner;
            this.user = user;
            RightEarId = rightEarId;
            LeftEarId = leftEarId;
            NickName = nickName;
            Race = race;
            Color = color;
            ApprovedRaceColorCombination = approvedRaceColorCombination;
            DateOfBirth = dateOfBirth;
            DateOfDeath = dateOfDeath;
            Gender = gender;
            IsPublic = isPublic;
        }

        public Rabbit() { } 

    }
}
