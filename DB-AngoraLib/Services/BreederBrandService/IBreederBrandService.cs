using DB_AngoraLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Services.BreederBrandService
{
    public interface IBreederBrandService
    {
        Task<BreederBrand> Create_BreederBrand(string userId);
        Task<BreederBrand> Get_BreederBrandById(int id);
        Task<BreederBrand> Get_BreederBrandByUserId(string userId);

        Task Update_BreederBrand(BreederBrand breederBrand);
        Task Delete_BreederBrand(int id);
    }
}
