using DB_AngoraLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DB_AngoraLib.Services.ValidationService
{

    public class RabbitValidator
    {
        public static readonly Dictionary<Race, List<Color>> NotApprovedColorsByRace = new Dictionary<Race, List<Color>>();

        public RabbitValidator()
        {
            // Tilføj farver, der ikke er godkendt for hver race
            NotApprovedColorsByRace[Race.Angora] = new List<Color> { Color.LysGråblå_Gouwenaar, Color.LyseBlå_BlåBeveren, Color.Ræverød_NewZealandRed, Color.Sallander, };
            NotApprovedColorsByRace[Race.Satinangora] = new List<Color> { Color.LysGråblå_Gouwenaar, Color.LyseBlå_BlåBeveren, Color.Ræverød_NewZealandRed, Color.Sallander, };
            NotApprovedColorsByRace[Race.Satin] = new List<Color> { Color.Hvid_Albino, };
            NotApprovedColorsByRace[Race.Rex] = new List<Color> { Color.Vildtrød_Harefarvet, Color.Gulrød_Bourgogne, Color.Ræverød_NewZealandRed, Color.LyseBlå_BlåBeveren, Color.Gråblå_LilleEgern, Color.Gråblå_MarburgerEgern, Color.LysGråblå_Gouwenaar, Color.Jerngrå, };
            NotApprovedColorsByRace[Race.Lille_Rex] = new List<Color> { Color.Vildtrød_Harefarvet, Color.Gulrød_Bourgogne, Color.Ræverød_NewZealandRed, Color.LyseBlå_BlåBeveren, Color.Gråblå_LilleEgern, Color.Gråblå_MarburgerEgern, Color.LysGråblå_Gouwenaar, Color.Jerngrå, };
        }


        public bool ValidateRaceAndColorCombo(Race race, Color color)
        {
            if (NotApprovedColorsByRace.TryGetValue(race, out var notApprovedColors))
            {
                return !notApprovedColors.Contains(color);
            }
            return false;
        }


        public void ValidateRace(Race race)
        {
            if (!Enum.IsDefined(typeof(Race), race))
            {
                throw new ArgumentException("Kanin.Race: Vælg en gyldig race fra listen");
            }
        }


        public void ValidateColor(Color color)
        {
            if (!Enum.IsDefined(typeof(Color), color))
            {
                throw new ArgumentException("Ugyldig farve! Vælg fra listen");
            }
        }


        public void ValidateRightEarId(string rightEarId)
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

        public void ValidateLeftEarId(string leftEarId)
        {

            Regex threeToFiveNumbersDigit = new Regex(@"^\d{3,5}$"); // 3-5 numeriske tal

            if (string.IsNullOrEmpty(leftEarId))
            {
                throw new ArgumentNullException("Kanin.Id: Feldtet skal udfyldes");
            }

            if (!threeToFiveNumbersDigit.IsMatch(leftEarId))
            {
                throw new ArgumentException("Kanin.Id, skal være imellem 3 og 5 numeriske tal");
            }
        }


        public void ValidateGender(Gender gender)
        {
            if (!Enum.IsDefined(typeof(Gender), gender))
            {
                throw new ArgumentException("Kanin.Køn: Vælg et gyldigt køn (Han/Hun)");
            }
        }


        public void ValidateParentId(string parentId)
        {
            // Hvis parentId er null eller tom, returner uden fejl
            if (string.IsNullOrEmpty(parentId))
            {
                return;
            }

            Regex parentIdPattern = new Regex(@"^\d{4}-\d{3,5}$");

            if (!parentIdPattern.IsMatch(parentId))
            {
                throw new ArgumentException("Kanin.ForældreId: Skal være i formatet RightEarId-LeftEarId (f.eks. 5095-021)");
            }
        }


        public void ValidateRabbit(Rabbit rabbit)
        {
            //Key validations
            ValidateRightEarId(rabbit.RightEarId);
            ValidateLeftEarId(rabbit.LeftEarId);

            ValidateRace(rabbit.Race);
            ValidateColor(rabbit.Color);
            ValidateRaceAndColorCombo(rabbit.Race, rabbit.Color);
            ValidateGender(rabbit.Gender);

            //ValidateParentId(rabbit.FatherId);
            //ValidateParentId(rabbit.MotherId);
        }

    }
}
