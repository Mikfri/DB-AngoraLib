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

            ///////////////////////// Ida's kaniner

            new Rabbit(
            "5095",                 // RightEarId
            "002",                  // LeftEarId
            "5095",                 // User.Id
            "Sov",                  // Nickname
            Race.Angora,
            Color.Blå,
            //null,                   //ApprovedRaceColorCombination
            new DateOnly(2020, 06, 12), // Format: yyyy-MM-dd
            new DateOnly(2022, 07, 22), // Format: yyyy-MM-dd
            Gender.Hun,         
            IsPublic.No),

            new Rabbit(
            "5095",
            "001",
            "5095",
            "Kaliba",
            Race.Angora,
            Color.LilleEgern_Gråblå,
            //null,
            new DateOnly(2019, 02, 27),
            new DateOnly(2024, 04, 13),
            Gender.Hun,
            IsPublic.No),

            new Rabbit(
            "5095",
            "003",
            "5095",
            "Smørklat Smør",
            Race.Angora,
            Color.Gulbrun_Isabella,
            new DateOnly(2020, 03, 12),
            new DateOnly(2023, 11, 3),
            Gender.Hun,            
            IsPublic.No),

            new Rabbit(
            "4640",
            "120",
            "5095",
            "Mulan",
            Race.Angora,
            Color.Blå,
            new DateOnly(2021, 05, 11),
            new DateOnly(2023, 11, 3),
            Gender.Hun,
            IsPublic.No),

            new Rabbit(
            "4640",
            "105",
            "5095",
            "Ingolf",
            Race.Angora,
            Color.Chinchilla,
            new DateOnly(2021, 04, 05),
            null,
            Gender.Han,
            IsPublic.No),

            ///////////////////////// Maja's kaniner
            new Rabbit(
            "4398",
            "3020",
            "5053",
            "Douglas",
            Race.Lille_Rex,
            Color.Gulbrun_Isabella,
            //null,
            new DateOnly(2022, 07, 22),
            null,
            Gender.Han,
            IsPublic.No),

            new Rabbit(
            "5053",
            "0223",
            "5053",
            "Chinchou",
            Race.Satinangora,
            Color.Blå,
            new DateOnly(2023, 05, 30),
            null,
            Gender.Hun,
            IsPublic.No),

            new Rabbit(
            "5053",
            "0723",
            "5053",
            "Sandshrew",
            Race.Satinangora,
            Color.Brun_Havana,
            new DateOnly(2024, 10, 15),
            null,
            Gender.Han,
            IsPublic.No),

            new Rabbit(
            "5053",
            "1023",
            "5053",
            "Marabou",
            Race.Satinangora,
            Color.Brun_Havana,
            new DateOnly(2024, 10, 15),
            null,
            Gender.Hun,
            IsPublic.No),

            new Rabbit(
            "M63",
            "2104",
            "5053",
            "Ortovi",
            Race.Satinangora,
            Color.Elfenben,
            new DateOnly(2023, 05, 22),
            null,
            Gender.Hun,
            IsPublic.No),

            new Rabbit(
            "5053",
            "0823",
            "5053",
            "Pepsi",
            Race.Satinangora,
            Color.Rødbrun_Madagascar,
            new DateOnly(2024, 10, 15),
            null,
            Gender.Hun,
            IsPublic.No),

            new Rabbit(
            "5053",
            "0923",
            "5053",
            "Cola",
            Race.Satinangora,
            Color.Rødbrun_Madagascar,
            new DateOnly(2024, 10, 15),
            new DateOnly(2024, 03, 14),
            Gender.Han,
            IsPublic.No),

            new Rabbit(
            "5053",
            "0623",
            "5053",
            "Karla",
            Race.Satinangora,
            Color.MarburgerEgern_Gråblå,
            new DateOnly(2023, 08, 17),
            null,
            Gender.Hun,
            IsPublic.No),

            new Rabbit(
            "M63",
            "3102",
            "5053",
            "Xådda",
            Race.Satinangora,
            Color.MarburgerEgern_Gråblå,
            new DateOnly(2023, 09, 23),
            null,
            Gender.Hun,
            IsPublic.No),

            new Rabbit(
            "4977",
            "206",
            "5053",
            "Dario",
            Race.Satinangora,
            Color.Sort_Alaska,
            new DateOnly(2022, 02, 02),
            null,
            Gender.Han,
            IsPublic.No),

            new Rabbit(
            "4977",
            "315",
            "5053",
            "Miranda",
            Race.Satinangora,
            Color.Sort_Alaska,
            new DateOnly(2023, 01, 13),
            null,
            Gender.Hun,
            IsPublic.No),

            new Rabbit(
            "5053",
            "0423",
            "5053",
            "Gastly",
            Race.Satinangora,
            Color.Sort_Alaska,
            new DateOnly(2023, 05, 30),
            null,
            Gender.Hun,
            IsPublic.No),

            new Rabbit(
            "V23",
            "023",
            "5053",
            "Aslan",
            Race.Satinangora,
            Color.Vildtbrun,
            new DateOnly(2020, 04, 10),
            new DateOnly(2024, 04, 23),
            Gender.Han,
            IsPublic.No),

            new Rabbit(
            "4977",
            "213",
            "5053",
            "Frida",
            Race.Satinangora,
            Color.Vildtrød_Harefarvet,
            new DateOnly(2022, 03, 24),
            null,
            Gender.Hun,
            IsPublic.No),
        };

        public static List<Rabbit> GetMockRabbits()
        { return _rabbitsList; }

    }
}
