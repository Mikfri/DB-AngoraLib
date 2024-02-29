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
        Hun,    // NiceToHave: ♂    0.1
        Han,    // NiceToHave: ♀    1.0
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
        Stikkelhår_Trønder,

        // Endnu ikke godkendte farver i DK
        Elfenben,
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
        public string RightEarId { get; set; }

        public string LeftEarId { get; set; }

        public string? Owner { get; set; }
        public virtual User User { get; set; } // virtual -> lazy loading (færre DB requests)

        public string? NickName { get; set; }

        public Race Race { get; set; }

        public Color Color { get; set; }

        public bool? ApprovedRaceColorCombination { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public DateOnly? DateOfDeath { get; set; }

        public Gender Gender { get; set; }
        public int? MotherId { get; set; }      // todo: Skal kædes til kaninens comp-key. ala: "4977-206"
        public int? FatherId { get; set; }
        public IsPublic? IsPublic { get; set; }


        public Rabbit(string rightEarId, string leftEarId, string? owner, string? nickName, Race race, Color color, bool? approvedRaceColorCombination, DateOnly dateOfBirth, DateOnly? dateOfDeath, Gender gender, int? motherId, int? fatherId, IsPublic? isPublic)
        {
            RightEarId = rightEarId;
            LeftEarId = leftEarId;
            Owner = owner;
            NickName = nickName;
            Race = race;
            Color = color;
            ApprovedRaceColorCombination = approvedRaceColorCombination;
            DateOfBirth = dateOfBirth;
            DateOfDeath = dateOfDeath;
            Gender = gender;
            MotherId = motherId;
            FatherId = fatherId;
            IsPublic = isPublic;
        }

        public Rabbit() { }

    }
}
