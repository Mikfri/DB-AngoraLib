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


        public bool ValidateRaceAndColorCombo(Rabbit rabbit)
        {
            Race race = rabbit.Race;
            Color color = rabbit.Color;

            if (RabbitValidator.NotApprovedColorsByRace.TryGetValue(race, out var selectedColor))
            {
                return !selectedColor.Contains(color);
            }
            else
            {
                return false;
            }
        }


        public void ValidateRace(Rabbit rabbit)
        {
            string raceStr = rabbit.Race.ToString();

            if (string.IsNullOrEmpty(raceStr))
            {
                throw new ArgumentNullException("Kanin.Race: Feldtet skal udfyldes");
            }

            if (!Enum.IsDefined(typeof(Race), rabbit.Race))
            {
                throw new ArgumentException("Kanin.Race: Vælg en gyldig race fra listen");
            }
        }

        public void ValidateColor(Rabbit rabbit)
        {
            Color color = rabbit.Color;

            if (!Enum.IsDefined(typeof(Color), color))
            {
                throw new ArgumentException("Ugyldig farve! Vælg fra listen");
            }
        }

        public void ValidateRightEarId(Rabbit rabbit)
        {
            string rightEarId = rabbit.RightEarId;

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

        public void ValidateLeftEarId(Rabbit rabbit)
        {
            string leftEarId = rabbit.LeftEarId;

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


        public void ValidateGender(Rabbit rabbit)
        {
            string genderStr = rabbit.Gender.ToString();

            if (string.IsNullOrEmpty(genderStr))
            {
                throw new ArgumentNullException("Kanin.Køn: Feldtet skal udfyldes");
            }

            if (!Enum.IsDefined(typeof(Gender), rabbit.Gender))
            {
                throw new ArgumentException("Kanin.Køn: Vælg et gyldigt køn (Han/Hun)");
            }
        }

        //public void ValidateParentIds(Rabbit rabbit)
        //{
        //    if ((string.IsNullOrEmpty(rabbit.MotherRightEarId) && !string.IsNullOrEmpty(rabbit.MotherLeftEarId)) ||
        //        (!string.IsNullOrEmpty(rabbit.MotherRightEarId) && string.IsNullOrEmpty(rabbit.MotherLeftEarId)))
        //    {
        //        throw new ArgumentException("Kanin.ModerId: Begge ører ID er nødvendige for at specificere moderen");
        //    }

        //    if ((string.IsNullOrEmpty(rabbit.FatherRightEarId) && !string.IsNullOrEmpty(rabbit.FatherLeftEarId)) ||
        //        (!string.IsNullOrEmpty(rabbit.FatherRightEarId) && string.IsNullOrEmpty(rabbit.FatherLeftEarId)))
        //    {
        //        throw new ArgumentException("Kanin.FaderId: Begge ører ID er nødvendige for at specificere faderen");
        //    }
        //}

        public void ValidateRabbit(Rabbit rabbit)
        {
            //Key validations
            ValidateRightEarId(rabbit);
            ValidateLeftEarId(rabbit);

            ValidateRace(rabbit);
            ValidateColor(rabbit);
            ValidateRaceAndColorCombo(rabbit);
            ValidateGender(rabbit);
            //ValidateParentIds(rabbit);
        }

    }
}
