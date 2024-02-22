using DB_AngoraLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DB_AngoraLib.Services.ValidationService
{

    public class ValidatorService
    {
        private readonly Dictionary<Race, List<Color>> NotApprovedColorsByRace;

        public ValidatorService()
        {
            NotApprovedColorsByRace = new Dictionary<Race, List<Color>>();

            // Tilføj farver, der ikke er godkendt for hver race
            NotApprovedColorsByRace[Race.Angora] = new List<Color> { Color.Chinchilla, Color.Gouwenaar, Color.LyseBlå_BlåBeveren, Color.Ræverød_NewZealandRed, Color.Sallander, };
            NotApprovedColorsByRace[Race.Satinangora] = new List<Color> { Color.Chinchilla, Color.Gouwenaar, Color.LyseBlå_BlåBeveren, Color.Ræverød_NewZealandRed, Color.Sallander, };
            NotApprovedColorsByRace[Race.Satin] = new List<Color> { Color.Hvid, };
            NotApprovedColorsByRace[Race.Rex] = new List<Color> { Color.Vildtrød_Harefarvet, Color.Gulrød_Bourgogne, Color.Ræverød_NewZealandRed, Color.LyseBlå_BlåBeveren, Color.LilleEgern_Gråblå, Color.MarburgerEgern_Gråblå, Color.Gouwenaar, Color.Jerngrå, };
            NotApprovedColorsByRace[Race.Lille_Rex] = new List<Color> { Color.Vildtrød_Harefarvet, Color.Gulrød_Bourgogne, Color.Ræverød_NewZealandRed, Color.LyseBlå_BlåBeveren, Color.LilleEgern_Gråblå, Color.MarburgerEgern_Gråblå, Color.Gouwenaar, Color.Jerngrå, };
        }

        

        public bool ValidateRaceAndColorCombo(Rabbit rabbit)
        {
            Race? race = rabbit.Race;
            Color? color = rabbit.Color;

            if (race.HasValue && NotApprovedColorsByRace.TryGetValue(race.Value, out var notApprovedColors))
            {
                bool noColorAndRace = !color.HasValue || !notApprovedColors.Contains(color.Value);

                if (noColorAndRace)
                {
                    rabbit.ApprovedRaceColorCombination = false;
                }

                return noColorAndRace; //fordi jeg har gjort race og color nullable
            }

            return true;
        }

        public void ValidateRace(Rabbit rabbit)
        {
            Race? race = rabbit.Race;

            if (!Enum.IsDefined(typeof(Race), race))
            {
                throw new ArgumentException("Ugyldig race! Vælg fra listen");
            }
        }

        public void ValidateColor(Rabbit rabbit)
        {
            Color? color = rabbit.Color;

            if (!Enum.IsDefined(typeof(Color), color))
            {
                throw new ArgumentException("Ugyldig farve! Vælg fra listen");
            }
        }

        public void ValidateLeftEarId(Rabbit rabbit)
        {
            string leftEarId = rabbit.LeftEarId;

            Regex threeToFiveNumbersDigit = new Regex(@"^\d{3,5}$");

            if (string.IsNullOrEmpty(leftEarId))
            {
                throw new ArgumentNullException("Kanin.Id: Skal have en værdi");
            }

            if (!threeToFiveNumbersDigit.IsMatch(leftEarId))
            {
                throw new ArgumentException("Kanin.Id, skal være 4 numeriske tal");
            }
        }

        public void ValidateRightEarId(Rabbit rabbit)
        {
            string rightEarId = rabbit.RightEarId;

            Regex fourNumbersDigit = new Regex(@"^\d{4}$");

            if (string.IsNullOrEmpty(rightEarId))
            {
                throw new ArgumentNullException("Kanin.Id: Skal have en værdi");
            }

            if (!fourNumbersDigit.IsMatch(rightEarId))
            {
                throw new ArgumentException("Kanin.Id, skal være 4 numeriske tal");
            }
        }

        public void ValidateRabbit(Rabbit rabbit)
        {
            ValidateLeftEarId(rabbit);
            ValidateRightEarId(rabbit);
            ValidateRace(rabbit);
            ValidateColor(rabbit);
            ValidateRaceAndColorCombo(rabbit);
        }

    }
}
