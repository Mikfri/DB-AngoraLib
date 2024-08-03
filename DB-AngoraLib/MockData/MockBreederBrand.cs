using DB_AngoraLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.MockData
{
    public class MockBreederBrand
    {
        public static List<BreederBrand> GetMockBreederBrands()
        {
            return new List<BreederBrand>
            {
                new BreederBrand
                {
                    Id = 1,
                    UserId = "IdasId",
                    BreederBrandName = "Den brogede Angora",
                    BreederBrandDescription = "Satin angora opdrætter, og producent af uld i forskellige plantefarver",
                    BreederBrandLogo = null
                },
                new BreederBrand
                {
                    Id = 2,
                    UserId = "MajasId",
                    BreederBrandName = "Slettens Angora",
                    BreederBrandDescription = "Oprdræt af Satin angoraer, samt salg af skind og uld",
                    BreederBrandLogo = "logo.png"
                }
            };
        }
    }
}
