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

        public string? OwnerId { get; set; }
        public User Owner { get; set; } // public virtual -> lazy loading (færre DB requests)

        public string? NickName { get; set; }
        public Race Race { get; set; }
        public Color Color { get; set; }
        public bool? ApprovedRaceColorCombination { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public DateOnly? DateOfDeath { get; set; }
        public Gender Gender { get; set; }

        public string? MotherRightEarId { get; set; }
        public string? MotherLeftEarId { get; set; }
        //[ForeignKey("MotherRightEarId, MotherLeftEarId")] // Overflødigt, da vi sætter det op i DbContext
        public virtual Rabbit? Mother { get; set; }

        public string? FatherRightEarId { get; set; }
        public string? FatherLeftEarId { get; set; }
        //[ForeignKey("FatherRightEarId, FatherLeftEarId")] // Uden dette gøres klassen u-afhængig af DbContext
        public virtual Rabbit? Father { get; set; }

        public IsPublic? IsPublic { get; set; }

        public virtual ICollection<Litter> Litters { get; set; }


        public Rabbit(string rightEarId, string leftEarId, string? ownerId, string? nickName, Race race, Color color, /*bool? approvedRaceColorCombination,*/ DateOnly dateOfBirth, DateOnly? dateOfDeath, Gender gender, string? motherRightEarId, string? motherLeftEarId, string? fatherRightEarId, string? fatherLeftEarId, IsPublic? isPublic)
        {
            RightEarId = rightEarId;
            LeftEarId = leftEarId;
            OwnerId = ownerId;
            NickName = nickName;
            Race = race;
            Color = color;
            /*ApprovedRaceColorCombination = approvedRaceColorCombination;*/
            DateOfBirth = dateOfBirth;
            DateOfDeath = dateOfDeath;
            Gender = gender;

            MotherRightEarId = motherRightEarId;
            MotherLeftEarId = motherLeftEarId;
            FatherRightEarId = fatherRightEarId;
            FatherLeftEarId = fatherLeftEarId;

            IsPublic = isPublic;
        }

        public Rabbit() { }

        public override string ToString()
        {
            return $"RightEarId: {RightEarId}, LeftEarId: {LeftEarId}, Owner: {Owner}, NickName: {NickName}, Race: {Race}, Color: {Color}, ApprovedRaceColorCombination: {ApprovedRaceColorCombination}, DateOfBirth: {DateOfBirth}, Gender: {Gender}";
        }
    }
}
