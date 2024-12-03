using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using DB_AngoraLib.Services.ValidationService;
using System.Text.RegularExpressions;

namespace DB_AngoraLib.Models
{
    public enum IsPublic
    {
        Nej,
        Ja
    }   

    public enum Gender
    {
        Doe,    // 0,  NiceToHave: ♂    0.1
        Buck,   // 1,  NiceToHave: ♀    1.0
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
        Satin_Angora,

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
        Rødbrungråblå_Lux,
        Gulrød_Bourgogne,
        Orange,
        Ræverød_NewZealandRed,
        Lutino_Vildtgul,
        Lutino_Shadow,
        Chinchilla,
        Schwarzgrannen,

        // Ensfarvede
        Sort_Alaska,
        Blå,
        LyseBlå_BlåBeveren,
        Gråblå_LilleEgern,
        Gråblå_MarburgerEgern,
        LysGråblå_Gouwenaar,
        Brun_Havana,
        Beige,
        Rødorange_Sachsengold,
        Hvid_Albino,            
        Hvid_Blåøjet,

        // Ensfarvede m. slør
        Rødbrun_Madagascar,
        Kanel,
        Gulbrun_Isabella,
        Sallander,

        // Ensfarvede m. stikkelhår
        Sølv,
        Stikkelhår_Trønder,

        //Special farver
        Elfenben = Hvid_Albino
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
        public static readonly Dictionary<Race, List<Color>> NotApprovedColorsByRace = new Dictionary<Race, List<Color>>();

        private string rightEarId;
        private string leftEarId;

        public string EarCombId { get; private set; }

        public string RightEarId
        {
            get => rightEarId;
            set
            {
                rightEarId = value;
                UpdateEarCombId();
            }
        }

        public string LeftEarId
        {
            get => leftEarId;
            set
            {
                leftEarId = value;
                UpdateEarCombId();
            }
        }

        public string? OriginId { get; set; }
        public Breeder? UserOrigin { get; set; }


        public string? OwnerId { get; set; }
        public Breeder? UserOwner { get; set; } // public virtual -> lazy loading (færre DB requests)

        public string? NickName { get; set; }
        public Race Race { get; set; }
        public Color Color { get; set; }

        public bool ApprovedRaceColorCombination
        {
            get
            {
                if (NotApprovedColorsByRace.TryGetValue(Race, out var notApprovedColors))
                {
                    return !notApprovedColors.Contains(Color);
                }
                else
                {
                    return false;
                }
            }
        }

        public DateOnly DateOfBirth { get; set; }
        public bool IsJuvenile
        {
            get
            {
                var ageInWeeks = (DateTime.Now.Date - DateOfBirth.ToDateTime(TimeOnly.MinValue)).TotalDays / 7;
                return ageInWeeks >= 8 && ageInWeeks <= 14;
            }
        }
        public DateOnly? DateOfDeath { get; set; }
        
        public Gender Gender { get; set; }
        public IsPublic ForSale { get; set; }       // TODO: Kunne være DateOnly, så man kan se hvornår den blev sat til salg + ny property for pris og DateOnly for solgt dato
        public IsPublic ForBreeding { get; set; }

        public string? FatherId_Placeholder { get; set; }
        public string? Father_EarCombId { get; set; }
        public Rabbit? Father { get; set; }

        public string? MotherId_Placeholder { get; set; }
        public string? Mother_EarCombId { get; set; }
        public Rabbit? Mother { get; set; }

        public string? ProfilePic { get; set; } // Path or URI to the profile picture

        public virtual ICollection<Rabbit> MotheredChildren { get; set; } // Er altid null hvis, Rabbit er far/Han
        public virtual ICollection<Rabbit> FatheredChildren { get; set; } // Er altid null hvis, Rabbit er mor/Hun
        public virtual ICollection<Photo> Photos { get; set; }
        //public virtual ICollection<RabbitTransfer> PreviousOwners { get; set; }
        public virtual ICollection<Trimming> Trimmings { get; set; }



        public Rabbit(string rightEarId, string leftEarId, string? originId, string? ownerId, string? nickName, Race race, Color color, DateOnly dateOfBirth, DateOnly? dateOfDeath, Gender gender, IsPublic forSale, IsPublic forBreeding, string? fatherId_Placeholder, string? motherId_Placeholder)
        {
            EarCombId = $"{rightEarId}-{leftEarId}";

            RightEarId = rightEarId;
            LeftEarId = leftEarId;
            OriginId = originId;
            OwnerId = ownerId;
            NickName = nickName;
            Race = race;
            Color = color;
            DateOfBirth = dateOfBirth;
            DateOfDeath = dateOfDeath;
            Gender = gender;

            ForSale = forSale;
            ForBreeding = forBreeding;

            Father_EarCombId = fatherId_Placeholder;
            Mother_EarCombId = motherId_Placeholder;

        }
          

        public Rabbit() 
        {
            NotApprovedColorsByRace[Race.Angora] = new List<Color> { Color.LysGråblå_Gouwenaar, Color.LyseBlå_BlåBeveren, Color.Ræverød_NewZealandRed, Color.Sallander, };
            NotApprovedColorsByRace[Race.Satin_Angora] = new List<Color> { Color.LysGråblå_Gouwenaar, Color.LyseBlå_BlåBeveren, Color.Ræverød_NewZealandRed, Color.Sallander, };
            NotApprovedColorsByRace[Race.Satin] = new List<Color> { Color.Hvid_Albino, };
            NotApprovedColorsByRace[Race.Rex] = new List<Color> { Color.Vildtrød_Harefarvet, Color.Gulrød_Bourgogne, Color.Ræverød_NewZealandRed, Color.LyseBlå_BlåBeveren, Color.Gråblå_LilleEgern, Color.Gråblå_MarburgerEgern, Color.LysGråblå_Gouwenaar, Color.Jerngrå, };
            NotApprovedColorsByRace[Race.Lille_Rex] = new List<Color> { Color.Vildtrød_Harefarvet, Color.Gulrød_Bourgogne, Color.Ræverød_NewZealandRed, Color.LyseBlå_BlåBeveren, Color.Gråblå_LilleEgern, Color.Gråblå_MarburgerEgern, Color.LysGråblå_Gouwenaar, Color.Jerngrå, };

        }

        private void UpdateEarCombId()
        {
            EarCombId = $"{RightEarId}-{LeftEarId}";
        }

        public bool ValidateRaceAndColorCombo(Race race, Color color)
        {
            if (NotApprovedColorsByRace.TryGetValue(race, out var notApprovedColors))
            {
                return !notApprovedColors.Contains(color);
            }
            return false;
        }


        public void ValidateRace()
        {
            if (!Enum.IsDefined(typeof(Race), Race))
            {
                throw new ArgumentException("Kanin.Race: Vælg en gyldig race fra listen");
            }
        }


        public void ValidateColor()
        {
            if (!Enum.IsDefined(typeof(Color), Color))
            {
                throw new ArgumentException("Ugyldig farve! Vælg fra listen");
            }
        }


        public void ValidateRightEarId()
        {
            //string rightEarId = rabbit.RightEarId;

            Regex fourNumbersDigit = new Regex(@"^\d{4}$");

            if (string.IsNullOrEmpty(rightEarId))
            {
                throw new ArgumentNullException("Kanin.AvlerNo: Feldtet skal udfyldes");
            }

            if (!fourNumbersDigit.IsMatch(rightEarId))
            {
                throw new ArgumentException("Kanin.AvlerNo: Skal være 4 numeriske tal");
            }
        }

        public void ValidateLeftEarId()
        {

            Regex threeToSixNumbersDigit = new Regex(@"^\d{3,6}$"); // 3-6 numeriske tal

            if (string.IsNullOrEmpty(leftEarId))
            {
                throw new ArgumentNullException("Kanin.Id: Feldtet skal udfyldes");
            }

            if (!threeToSixNumbersDigit.IsMatch(leftEarId))
            {
                throw new ArgumentException("Kanin.Id, skal være imellem 3 og 6 numeriske tal");
            }
        }


        public void ValidateGender()
        {
            if (!Enum.IsDefined(typeof(Gender), Gender))
            {
                throw new ArgumentException("Kanin.Køn: Vælg et gyldigt køn. Buck(han) eller Doe(hun)");
            }
        }


        public void ValidateParentId()
        {
            // Hvis parentId er null eller tom, returner uden fejl
            if (string.IsNullOrEmpty(FatherId_Placeholder) && string.IsNullOrEmpty(MotherId_Placeholder))
            {
                return;
            }

            Regex parentIdPattern = new Regex(@"^\d{4}-\d{3,6}$");

            if (!string.IsNullOrEmpty(FatherId_Placeholder) && !parentIdPattern.IsMatch(FatherId_Placeholder))
            {
                throw new ArgumentException("Kanin.ForældreId: Skal være i formatet RightEarId-LeftEarId (f.eks. 5095-021)");
            }

            if (!string.IsNullOrEmpty(MotherId_Placeholder) && !parentIdPattern.IsMatch(MotherId_Placeholder))
            {
                throw new ArgumentException("Kanin.ForældreId: Skal være i formatet RightEarId-LeftEarId (f.eks. 5095-021)");
            }
        }

        public void ValidateRabbit()
        {
            //Key validations
            ValidateRightEarId();
            ValidateLeftEarId();

            ValidateRace();
            ValidateColor();
            ValidateGender();

            //ValidateRaceAndColorCombo();

        }

    }
}
