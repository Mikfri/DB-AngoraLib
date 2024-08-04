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
            new Rabbit
            {
                RightEarId = "5095",
                LeftEarId = "002",
                OriginId = "IdasId",
                OwnerId = "IdasId",
                NickName = "Sov",
                Race = Race.Angora,
                Color = Color.Blå,
                DateOfBirth = new DateOnly(2020, 06, 12),
                DateOfDeath = new DateOnly(2022, 07, 22),
                Gender = Gender.Doe,
                ForSale = IsPublic.Nej,
                ForBreeding = IsPublic.Nej
            },
            new Rabbit
            {
                RightEarId = "5095",
                LeftEarId = "001",
                OriginId = "IdasId",
                OwnerId = "IdasId",
                NickName = "Kaliba",
                Race = Race.Angora,
                Color = Color.Gråblå_LilleEgern,
                DateOfBirth = new DateOnly(2019, 02, 27),
                DateOfDeath = new DateOnly(2024, 04, 13),
                Gender = Gender.Doe,
                ForSale = IsPublic.Nej,
                ForBreeding = IsPublic.Nej
            },
            new Rabbit
            {
                RightEarId = "5095",
                LeftEarId = "003",
                OriginId = "IdasId",
                OwnerId = "IdasId",
                NickName = "Smørklat Smør",
                Race = Race.Angora,
                Color = Color.Gulbrun_Isabella,
                DateOfBirth = new DateOnly(2020, 03, 12),
                DateOfDeath = new DateOnly(2023, 11, 3),
                Gender = Gender.Doe,
                ForSale = IsPublic.Nej,
                ForBreeding = IsPublic.Nej
            },
            new Rabbit
            {
                RightEarId = "4640",
                LeftEarId = "120",
                OriginId = null,
                OwnerId = "IdasId",
                NickName = "Mulan",
                Race = Race.Angora,
                Color = Color.Blå,
                DateOfBirth = new DateOnly(2021, 05, 11),
                DateOfDeath = new DateOnly(2023, 11, 3),
                Gender = Gender.Doe,
                ForSale = IsPublic.Nej,
                ForBreeding = IsPublic.Nej
            },
            new Rabbit
            {
                RightEarId = "4640",
                LeftEarId = "105",
                OriginId = null,
                OwnerId = "IdasId",
                NickName = "Ingolf",
                Race = Race.Angora,
                Color = Color.Chinchilla,
                DateOfBirth = new DateOnly(2021, 04, 05),
                DateOfDeath = null,
                Gender = Gender.Buck,
                ForSale = IsPublic.Nej,
                ForBreeding = IsPublic.Ja
            },
            new Rabbit
            {
                RightEarId = "5095",
                LeftEarId = "0124",
                OriginId = "IdasId",
                OwnerId = "IdasId",
                NickName = "Aron",
                Race = Race.Satin_Angora,
                Color = Color.Kanel,
                DateOfBirth = new DateOnly(2024, 05, 07),
                DateOfDeath = null,
                Gender = Gender.Buck,
                ForSale = IsPublic.Nej,
                ForBreeding = IsPublic.Ja
            },
            new Rabbit
            {
                RightEarId = "5095",
                LeftEarId = "0624",
                OriginId = "IdasId",
                OwnerId = "IdasId",
                NickName = "Articuno",
                Race = Race.Satin_Angora,
                Color = Color.Vildtblå_PerleEgern,
                DateOfBirth = new DateOnly(2024, 05, 07),
                DateOfDeath = null,
                Gender = Gender.Doe,
                ForSale = IsPublic.Nej,
                ForBreeding = IsPublic.Nej
            },
            new Rabbit
            {
                RightEarId = "5095",
                LeftEarId = "0224",
                OriginId = "IdasId",
                OwnerId = "IdasId",
                NickName = "Azelf",
                Race = Race.Satin_Angora,
                Color = Color.Vildtrød_Harefarvet,
                DateOfBirth = new DateOnly(2024, 05, 07),
                DateOfDeath = null,
                Gender = Gender.Doe,
                ForSale = IsPublic.Ja,
                ForBreeding = IsPublic.Ja
            },
            new Rabbit
            {
                RightEarId = "5095",
                LeftEarId = "0324",
                OriginId = "IdasId",
                OwnerId = "IdasId",
                NickName = "Arcanine",
                Race = Race.Satin_Angora,
                Color = Color.Vildtrød_Harefarvet,
                DateOfBirth = new DateOnly(2024, 05, 07),
                DateOfDeath = null,
                Gender = Gender.Buck,
                ForSale = IsPublic.Nej,
                ForBreeding = IsPublic.Nej
            },
            //---------------------: MAJAs RABBIT COLLECTION
            new Rabbit
            {
                RightEarId = "4398",
                LeftEarId = "3020",
                OriginId = null,
                OwnerId = "MajasId",
                NickName = "Douglas",
                Race = Race.Lille_Rex,
                Color = Color.Gulbrun_Isabella,
                DateOfBirth = new DateOnly(2022, 07, 22),
                DateOfDeath = null,
                Gender = Gender.Buck,
                ForSale = IsPublic.Nej,
                ForBreeding = IsPublic.Ja
            },
            new Rabbit
            {
                RightEarId = "5053",
                LeftEarId = "0223",
                OriginId = "MajasId",
                OwnerId = "MajasId",
                NickName = "Chinchou",
                Race = Race.Satin_Angora,
                Color = Color.Blå,
                DateOfBirth = new DateOnly(2023, 05, 30),
                DateOfDeath = null,
                Gender = Gender.Doe,
                ForSale = IsPublic.Nej,
                ForBreeding = IsPublic.Ja
            },
            new Rabbit
            {
                RightEarId = "5053",
                LeftEarId = "0723",
                OriginId = "MajasId",
                OwnerId = "MajasId",
                NickName = "Sandshrew",
                Race = Race.Satin_Angora,
                Color = Color.Brun_Havana,
                DateOfBirth = new DateOnly(2023, 10, 15),
                DateOfDeath = null,
                Gender = Gender.Buck,
                ForSale = IsPublic.Ja,
                ForBreeding = IsPublic.Ja
            },
            new Rabbit
            {
                RightEarId = "5053",
                LeftEarId = "1023",
                OriginId = "MajasId",
                OwnerId = "MajasId",
                NickName = "Marabou",
                Race = Race.Satin_Angora,
                Color = Color.Brun_Havana,
                DateOfBirth = new DateOnly(2023, 10, 15),
                DateOfDeath = null,
                Gender = Gender.Doe,
                ForSale = IsPublic.Ja,
                ForBreeding = IsPublic.Nej
            },
            new Rabbit
            {
                RightEarId = "M63",
                LeftEarId = "2104",
                OriginId = null,
                OwnerId = "MajasId",
                NickName = "Ortovi",
                Race = Race.Satin_Angora,
                Color = Color.Elfenben,
                DateOfBirth = new DateOnly(2023, 05, 22),
                DateOfDeath = null,
                Gender = Gender.Doe,
                ForSale = IsPublic.Nej,
                ForBreeding = IsPublic.Ja
            },
            new Rabbit
            {
                RightEarId = "5053",
                LeftEarId = "0823",
                OriginId = "MajasId",
                OwnerId = "MajasId",
                NickName = "Pepsi",
                Race = Race.Satin_Angora,
                Color = Color.Kanel,
                DateOfBirth = new DateOnly(2023, 10, 15),
                DateOfDeath = null,
                Gender = Gender.Doe,
                ForSale = IsPublic.Nej,
                ForBreeding = IsPublic.Ja
            },
            new Rabbit
            {
                RightEarId = "5053",
                LeftEarId = "0923",
                OriginId = "MajasId",
                OwnerId = "MajasId",
                NickName = "Cola",
                Race = Race.Satin_Angora,
                Color = Color.Rødbrun_Madagascar,
                DateOfBirth = new DateOnly(2023, 10, 15),
                DateOfDeath = null,
                Gender = Gender.Buck,
                ForSale = IsPublic.Nej,
                ForBreeding = IsPublic.Ja
            },
            new Rabbit
            {
                RightEarId = "5053",
                LeftEarId = "0623",
                OriginId = "MajasId",
                OwnerId = "MajasId",
                NickName = "Karla",
                Race = Race.Satin_Angora,
                Color = Color.Gråblå_MarburgerEgern,
                DateOfBirth = new DateOnly(2023, 08, 17),
                DateOfDeath = null,
                Gender = Gender.Doe,
                ForSale = IsPublic.Nej,
                ForBreeding = IsPublic.Nej
            },
            new Rabbit
            {
                RightEarId = "M63",
                LeftEarId = "3102",
                OriginId = null,
                OwnerId = "MajasId",
                NickName = "Xådda",
                Race = Race.Satin_Angora,
                Color = Color.Gråblå_MarburgerEgern,
                DateOfBirth = new DateOnly(2023, 09, 23),
                DateOfDeath = null,
                Gender = Gender.Doe,
                ForSale = IsPublic.Nej,
                ForBreeding = IsPublic.Ja
            },
            new Rabbit
            {
                RightEarId = "4977",
                LeftEarId = "206",
                OriginId = null,
                OwnerId = "MajasId",
                NickName = "Dario",
                Race = Race.Satin_Angora,
                Color = Color.Sort_Alaska,
                DateOfBirth = new DateOnly(2022, 02, 02),
                DateOfDeath = new DateOnly(2024, 04, 05),
                Gender = Gender.Buck,
                ForSale = IsPublic.Nej,
                ForBreeding = IsPublic.Nej
            },
            new Rabbit
            {
                RightEarId = "4977",
                LeftEarId = "315",
                OriginId = null,
                OwnerId = "MajasId",
                NickName = "Miranda",
                Race = Race.Satin_Angora,
                Color = Color.Sort_Alaska,
                DateOfBirth = new DateOnly(2023, 01, 13),
                DateOfDeath = new DateOnly(2024, 04, 15),
                Gender = Gender.Doe,
                ForSale = IsPublic.Nej,
                ForBreeding = IsPublic.Nej
            },
            new Rabbit
            {
                RightEarId = "5053",
                LeftEarId = "0423",
                OriginId = "MajasId",
                OwnerId = "MajasId",
                NickName = "Gastly",
                Race = Race.Satin_Angora,
                Color = Color.Sort_Alaska,
                DateOfBirth = new DateOnly(2023, 05, 30),
                DateOfDeath = null,
                Gender = Gender.Doe,
                ForSale = IsPublic.Nej,
                ForBreeding = IsPublic.Nej
            },
            new Rabbit
            {
                RightEarId = "V23",
                LeftEarId = "023",
                OriginId = null,
                OwnerId = "MajasId",
                NickName = "Aslan",
                Race = Race.Satin_Angora,
                Color = Color.Vildtbrun,
                DateOfBirth = new DateOnly(2020, 04, 10),
                DateOfDeath = new DateOnly(2024, 04, 23),
                Gender = Gender.Buck,
                ForSale = IsPublic.Nej,
                ForBreeding = IsPublic.Nej
            },
            new Rabbit
            {
                RightEarId = "4977",
                LeftEarId = "213",
                OriginId = null,
                OwnerId = "MajasId",
                NickName = "Frida",
                Race = Race.Satin_Angora,
                Color = Color.Vildtrød_Harefarvet,
                DateOfBirth = new DateOnly(2022, 03, 24),
                DateOfDeath = null,
                Gender = Gender.Doe,
                ForSale = IsPublic.Nej,
                ForBreeding = IsPublic.Ja
            },
            new Rabbit
            {
                RightEarId = "5053",
                LeftEarId = "0120",
                OriginId = "MajasId",
                OwnerId = "MajasId",
                NickName = "Ulla",
                Race = Race.Angora,
                Color = Color.Gulbrun_Isabella,
                DateOfBirth = new DateOnly(2020, 03, 25),
                DateOfDeath = new DateOnly(2021, 05, 31),
                Gender = Gender.Doe,
                ForSale = IsPublic.Nej,
                ForBreeding = IsPublic.Nej
            },
            new Rabbit
            {
                RightEarId = "3658",
                LeftEarId = "0819",
                OriginId = null,
                OwnerId = "MajasId",
                NickName = "Karina",
                Race = Race.Angora,
                Color = Color.Hvid_Albino,
                DateOfBirth = new DateOnly(2019, 05, 31),
                DateOfDeath = new DateOnly(2023, 01, 31),
                Gender = Gender.Doe,
                ForSale = IsPublic.Nej,
                ForBreeding = IsPublic.Nej
            },
            new Rabbit
            {
                RightEarId = "5053",
                LeftEarId = "0523",
                OriginId = "MajasId",
                OwnerId = "MajasId",
                NickName = "Charizard",
                Race = Race.Satin_Angora,
                Color = Color.Vildtrød_Harefarvet,
                DateOfBirth = new DateOnly(2023, 08, 17),
                DateOfDeath = null,
                Gender = Gender.Buck,
                ForSale = IsPublic.Nej,
                ForBreeding = IsPublic.Nej
            },
            new Rabbit
            {
                RightEarId = "5053",
                LeftEarId = "0123",
                OriginId = "MajasId",
                OwnerId = "MajasId",
                NickName = "Pichu",
                Race = Race.Satin_Angora,
                Color = Color.Vildtrød_Harefarvet,
                DateOfBirth = new DateOnly(2023, 05, 30),
                DateOfDeath = null,
                Gender = Gender.Doe,
                ForSale = IsPublic.Nej,
                ForBreeding = IsPublic.Nej
            },
            new Rabbit
            {
                RightEarId = "5053",
                LeftEarId = "0323",
                OriginId = "MajasId",
                OwnerId = "MajasId",
                NickName = "Hunter",
                Race = Race.Satin_Angora,
                Color = Color.Sort_Alaska,
                DateOfBirth = new DateOnly(2023, 08, 17),
                DateOfDeath = new DateOnly(2023, 12, 18),
                Gender = Gender.Buck,
                ForSale = IsPublic.Nej,
                ForBreeding = IsPublic.Nej
            },
            new Rabbit
            {
                RightEarId = "5053",
                LeftEarId = "0124",
                OriginId = "MajasId",
                OwnerId = "MajasId",
                NickName = "Rollo Darminatan",
                Race = Race.Satin_Angora,
                Color = Color.Vildtrød_Harefarvet,
                DateOfBirth = new DateOnly(2024, 04, 1),
                DateOfDeath = null,
                Gender = Gender.Buck,
                ForSale = IsPublic.Nej,
                ForBreeding = IsPublic.Nej
            },
            new Rabbit
            {
                RightEarId = "5053",
                LeftEarId = "0224",
                OriginId = "MajasId",
                OwnerId = "MajasId",
                NickName = "Chokolade",
                Race = Race.Satin_Angora,
                Color = Color.Gråblå_LilleEgern,
                DateOfBirth = new DateOnly(2024, 04, 18),
                DateOfDeath = null,
                Gender = Gender.Doe,
                ForSale = IsPublic.Nej,
                ForBreeding = IsPublic.Nej
            },
            new Rabbit
            {
                RightEarId = "5053",
                LeftEarId = "0324",
                OriginId = "MajasId",
                OwnerId = "MajasId",
                NickName = "Beartic",
                Race = Race.Satin_Angora,
                Color = Color.Gråblå_LilleEgern,
                DateOfBirth = new DateOnly(2024, 04, 18),
                DateOfDeath = null,
                Gender = Gender.Buck,
                ForSale = IsPublic.Nej,
                ForBreeding = IsPublic.Nej
            },
            new Rabbit
            {
                RightEarId = "5053",
                LeftEarId = "0524",
                OriginId = "MajasId",
                OwnerId = "MajasId",
                NickName = "Metchi",
                Race = Race.Satin_Angora,
                Color = Color.Brun_Havana,
                DateOfBirth = new DateOnly(2024, 04, 18),
                DateOfDeath = null,
                Gender = Gender.Buck,
                ForSale = IsPublic.Nej,
                ForBreeding = IsPublic.Nej
            },
            new Rabbit
            {
                RightEarId = "5053",
                LeftEarId = "0724",
                OriginId = "MajasId",
                OwnerId = "MajasId",
                NickName = "Dewgong",
                Race = Race.Satin_Angora,
                Color = Color.Gråblå_MarburgerEgern,
                DateOfBirth = new DateOnly(2024, 04, 18),
                DateOfDeath = null,
                Gender = Gender.Buck,
                ForSale = IsPublic.Nej,
                ForBreeding = IsPublic.Nej
            },
            new Rabbit
            {
                RightEarId = "5053",
                LeftEarId = "10724",
                OriginId = "MajasId",
                OwnerId = "MajasId",
                NickName = "Ice Beam",
                Race = Race.Satin_Angora,
                Color = Color.Gråblå_LilleEgern,
                DateOfBirth = new DateOnly(2024, 04, 18),
                DateOfDeath = null,
                Gender = Gender.Doe,
                ForSale = IsPublic.Nej,
                ForBreeding = IsPublic.Nej
            },
        };

        public static List<Rabbit> GetMockRabbits()
        {
            return _rabbitsList ?? new List<Rabbit>();
        }
    }
}