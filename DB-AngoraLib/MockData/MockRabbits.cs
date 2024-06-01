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
            1,
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
            ForSale.Nej,
            0,
            0),

            new Rabbit(
            2,
            "5095",
            "001",
            "IdasId",
            "Kaliba",
            Race.Angora,
            Color.Gråblå_LilleEgern,
            //null,
            new DateOnly(2019, 02, 27),
            new DateOnly(2024, 04, 13),
            Gender.Hun,
            ForSale.Ja,
            0,
            0),

            new Rabbit(
            3,
            "5095",
            "003",
            "IdasId",
            "Smørklat Smør",
            Race.Angora,
            Color.Gulbrun_Isabella,
            new DateOnly(2020, 03, 12),
            new DateOnly(2023, 11, 3),
            Gender.Hun,
            ForSale.Nej,
            0,
            0),

            new Rabbit(
            4,
            "4640",
            "120",
            "IdasId",
            "Mulan",
            Race.Angora,
            Color.Blå,
            new DateOnly(2021, 05, 11),
            new DateOnly(2023, 11, 3),
            Gender.Hun,
            ForSale.Nej,
            0,
            0),

            new Rabbit(
            5,
            "4640",
            "105",
            "IdasId",
            "Ingolf",
            Race.Angora,
            Color.Chinchilla,
            new DateOnly(2021, 04, 05),
            null,
            Gender.Han,
            ForSale.Ja,
            0,
            0),

            //---------------------: MAJAS RABBIT COLLECTION
            new Rabbit(
            6,
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
            ForSale.Ja,
            0,
            0),

            new Rabbit(
            7,
            "5053",
            "0223",
            "MajasId",
            "Chinchou",
            Race.Satinangora,
            Color.Blå,
            new DateOnly(2023, 05, 30),
            null,
            Gender.Hun,
            ForSale.Ja,
            0,
            0),

            new Rabbit(
            8,
            "5053",
            "0723",
            "MajasId",
            "Sandshrew",
            Race.Satinangora,
            Color.Brun_Havana,
            new DateOnly(2024, 10, 15),
            null,
            Gender.Han,
            ForSale.Ja,
            0,
            0),

            new Rabbit(
            9,
            "5053",
            "1023",
            "MajasId",
            "Marabou",
            Race.Satinangora,
            Color.Brun_Havana,
            new DateOnly(2024, 10, 15),
            null,
            Gender.Hun,
            ForSale.Nej,
            0,
            0),

            new Rabbit(
            10,
            "M63",
            "2104",
            "MajasId",
            "Ortovi",
            Race.Satinangora,
            Color.Elfenben,
            new DateOnly(2023, 05, 22),
            null,
            Gender.Hun,
            ForSale.Ja,
            0,
            0),

            new Rabbit(
            11,
            "5053",
            "0823",
            "MajasId",
            "Pepsi",
            Race.Satinangora,
            Color.Rødbrun_Madagascar,
            new DateOnly(2024, 10, 15),
            null,
            Gender.Hun,
            ForSale.Nej,
            0,
            0),

            new Rabbit(
            12,
            "5053",
            "0923",
            "MajasId",
            "Cola",
            Race.Satinangora,
            Color.Rødbrun_Madagascar,
            new DateOnly(2024, 10, 15),
            new DateOnly(2024, 03, 14),
            Gender.Han,
            ForSale.Nej,
            0,
            0),

            new Rabbit(
            13,
            "5053",
            "0623",
            "MajasId",
            "Karla",
            Race.Satinangora,
            Color.Gråblå_MarburgerEgern,
            new DateOnly(2023, 08, 17),
            null,
            Gender.Hun,
            ForSale.Nej,
            0,
            0),

            new Rabbit(
            14,
            "M63",
            "3102",
            "MajasId",
            "Xådda",
            Race.Satinangora,
            Color.Gråblå_MarburgerEgern,
            new DateOnly(2023, 09, 23),
            null,
            Gender.Hun,
            ForSale.Nej,
            0,
            0),

            new Rabbit(
            15,
            "4977",
            "206",
            "MajasId",
            "Dario",
            Race.Satinangora,
            Color.Sort_Alaska,
            new DateOnly(2022, 02, 02),
            null,
            Gender.Han,
            ForSale.Ja,
            0,
            0),

            new Rabbit(
            16,
            "4977",
            "315",
            "MajasId",
            "Miranda",
            Race.Satinangora,
            Color.Sort_Alaska,
            new DateOnly(2023, 01, 13),
            null,
            Gender.Hun,
            ForSale.Nej,
            0,
            0),

            new Rabbit(
            17,
            "5053",
            "0423",
            "MajasId",
            "Gastly",
            Race.Satinangora,
            Color.Sort_Alaska,
            new DateOnly(2023, 05, 30),
            null,
            Gender.Hun,
            ForSale.Nej,
            0,
            0),

            new Rabbit(
            18,
            "V23",
            "023",
            "MajasId",
            "Aslan",
            Race.Satinangora,
            Color.Vildtbrun,
            new DateOnly(2020, 04, 10),
            new DateOnly(2024, 04, 23),
            Gender.Han,
            ForSale.Ja,
            0,
            0),

            new Rabbit(
            19,
            "4977",
            "213",
            "MajasId",
            "Frida",
            Race.Satinangora,
            Color.Vildtrød_Harefarvet,
            new DateOnly(2022, 03, 24),
            null,
            Gender.Hun,
            ForSale.Ja,
            0,
            0),

            new Rabbit(
            20,
            "5053",
            "0120",
            "MajasId",
            "Ulla",
            Race.Angora,
            Color.Gulbrun_Isabella,
            new DateOnly(2020, 03, 25),
            new DateOnly(2021, 05, 31),
            Gender.Hun,
            ForSale.Nej,
            0,
            0),

            new Rabbit(
            21,
            "3658",
            "0819",
            "MajasId",
            "Karina",
            Race.Angora,
            Color.Hvid_Albino,
            new DateOnly(2019, 05, 31),
            new DateOnly(2023, 01, 31),
            Gender.Hun,
            ForSale.Nej,
            0,
            0),

            new Rabbit(
            22,
            "5053",
            "0523",
            "MajasId",
            "Charizard",
            Race.Satinangora,
            Color.Vildtrød_Harefarvet,
            new DateOnly(2023, 08, 17),
            null,
            Gender.Han,
            ForSale.Ja,
            0,
            0),
        };

        public static List<Rabbit> GetMockRabbits()
        { return _rabbitsList; }

    }
}
