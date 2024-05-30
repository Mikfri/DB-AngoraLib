using DB_AngoraLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.MockData
{
    public class MockRabbits
    {
        private static List<Rabbit> _rabbitsList = new List<Rabbit>()
        {

            //---------------------: IDAS RABBIT COLLECTION

            new Rabbit(
            "5095",                 // RightEarId
            "002",                  // LeftEarId
            "IdasId",               // User.Id
            "Sov",                  // Nickname
            Race.Angora,
            Color.Blå,
            //null,                   //ApprovedRaceColorCombination
            new DateOnly(2020, 06, 12), // Format: yyyy-MM-dd
            new DateOnly(2022, 07, 22), // Format: yyyy-MM-dd
            Gender.Hun,
            OpenProfile.Nej),

            new Rabbit(
            "5095",
            "001",
            "IdasId",
            "Kaliba",
            Race.Angora,
            Color.LilleEgern_Gråblå,
            //null,
            new DateOnly(2019, 02, 27),
            new DateOnly(2024, 04, 13),
            Gender.Hun,
            OpenProfile.Ja),

            new Rabbit(
            "5095",
            "003",
            "IdasId",
            "Smørklat Smør",
            Race.Angora,
            Color.Gulbrun_Isabella,
            new DateOnly(2020, 03, 12),
            new DateOnly(2023, 11, 3),
            Gender.Hun,            
            OpenProfile.Nej),

            new Rabbit(
            "4640",
            "120",
            "IdasId",
            "Mulan",
            Race.Angora,
            Color.Blå,
            new DateOnly(2021, 05, 11),
            new DateOnly(2023, 11, 3),
            Gender.Hun,
            OpenProfile.Nej),

            new Rabbit(
            "4640",
            "105",
            "IdasId",
            "Ingolf",
            Race.Angora,
            Color.Chinchilla,
            new DateOnly(2021, 04, 05),
            null,
            Gender.Han,
            OpenProfile.Ja),

            //---------------------: MAJAS RABBIT COLLECTION
            new Rabbit(
            "4398",
            "3020",
            "MajasId",
            "Douglas",
            Race.Lille_Rex,
            Color.Gulbrun_Isabella,
            //null,
            new DateOnly(2022, 07, 22),
            null,
            Gender.Han,
            OpenProfile.Ja),

            new Rabbit(
            "5053",
            "0223",
            "MajasId",
            "Chinchou",
            Race.Satinangora,
            Color.Blå,
            new DateOnly(2023, 05, 30),
            null,
            Gender.Hun,
            OpenProfile.Ja),

            new Rabbit(
            "5053",
            "0723",
            "MajasId",
            "Sandshrew",
            Race.Satinangora,
            Color.Brun_Havana,
            new DateOnly(2024, 10, 15),
            null,
            Gender.Han,
            OpenProfile.Ja),

            new Rabbit(
            "5053",
            "1023",
            "MajasId",
            "Marabou",
            Race.Satinangora,
            Color.Brun_Havana,
            new DateOnly(2024, 10, 15),
            null,
            Gender.Hun,
            OpenProfile.Nej),

            new Rabbit(
            "M63",
            "2104",
            "MajasId",
            "Ortovi",
            Race.Satinangora,
            Color.Elfenben,
            new DateOnly(2023, 05, 22),
            null,
            Gender.Hun,
            OpenProfile.Ja),

            new Rabbit(
            "5053",
            "0823",
            "MajasId",
            "Pepsi",
            Race.Satinangora,
            Color.Rødbrun_Madagascar,
            new DateOnly(2024, 10, 15),
            null,
            Gender.Hun,
            OpenProfile.Nej),

            new Rabbit(
            "5053",
            "0923",
            "MajasId",
            "Cola",
            Race.Satinangora,
            Color.Rødbrun_Madagascar,
            new DateOnly(2024, 10, 15),
            new DateOnly(2024, 03, 14),
            Gender.Han,
            OpenProfile.Nej),

            new Rabbit(
            "5053",
            "0623",
            "MajasId",
            "Karla",
            Race.Satinangora,
            Color.MarburgerEgern_Gråblå,
            new DateOnly(2023, 08, 17),
            null,
            Gender.Hun,
            OpenProfile.Nej),

            new Rabbit(
            "M63",
            "3102",
            "MajasId",
            "Xådda",
            Race.Satinangora,
            Color.MarburgerEgern_Gråblå,
            new DateOnly(2023, 09, 23),
            null,
            Gender.Hun,
            OpenProfile.Nej),

            new Rabbit(
            "4977",
            "206",
            "MajasId",
            "Dario",
            Race.Satinangora,
            Color.Sort_Alaska,
            new DateOnly(2022, 02, 02),
            null,
            Gender.Han,
            OpenProfile.Ja),

            new Rabbit(
            "4977",
            "315",
            "MajasId",
            "Miranda",
            Race.Satinangora,
            Color.Sort_Alaska,
            new DateOnly(2023, 01, 13),
            null,
            Gender.Hun,
            OpenProfile.Nej),

            new Rabbit(
            "5053",
            "0423",
            "MajasId",
            "Gastly",
            Race.Satinangora,
            Color.Sort_Alaska,
            new DateOnly(2023, 05, 30),
            null,
            Gender.Hun,
            OpenProfile.Nej),

            new Rabbit(
            "V23",
            "023",
            "MajasId",
            "Aslan",
            Race.Satinangora,
            Color.Vildtbrun,
            new DateOnly(2020, 04, 10),
            new DateOnly(2024, 04, 23),
            Gender.Han,
            OpenProfile.Ja),

            new Rabbit(
            "4977",
            "213",
            "MajasId",
            "Frida",
            Race.Satinangora,
            Color.Vildtrød_Harefarvet,
            new DateOnly(2022, 03, 24),
            null,
            Gender.Hun,
            OpenProfile.Ja),

            new Rabbit(
            "5053",
            "0120",
            "MajasId",
            "Ulla",
            Race.Angora,
            Color.Gulbrun_Isabella,
            new DateOnly(2020, 03, 25),
            new DateOnly(2021, 05, 31),
            Gender.Hun,
            OpenProfile.Nej),

            new Rabbit(
            "3658",
            "0819",
            "MajasId",
            "Karina",
            Race.Angora,
            Color.Hvid,
            new DateOnly(2019, 05, 31),
            new DateOnly(2023, 01, 31),
            Gender.Hun,
            OpenProfile.Nej),

            new Rabbit(
            "5053",
            "0523",
            "MajasId",
            "Charizard",
            Race.Satinangora,
            Color.Vildtrød_Harefarvet,
            new DateOnly(2023, 08, 17),
            null,
            Gender.Han,
            OpenProfile.Ja),
        };

        public static List<Rabbit> GetMockRabbits()
        { return _rabbitsList; }

    }
}
