using DB_AngoraLib.Models;
using DB_AngoraLib.Repository;
using DB_AngoraLib.Services.AccountService;
using DB_AngoraLib.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DB_AngoraLib.Services.RabbitService;

namespace DB_AngoraLib.Services.BreederBrandService
{
    public class BreederBrandServices : IBreederBrandService
    {
        private readonly IGRepository<BreederBrand> _dbRepository;
        private readonly IAccountService _accountServices;
        //private readonly IRabbitService _rabbitServices;

        public BreederBrandServices(IGRepository<BreederBrand> dbRepository, IAccountService accountService/*, RabbitServices rabbitServices*/)
        {
            _dbRepository = dbRepository;
            _accountServices = accountService;
            //_rabbitServices = rabbitServices;
        }

        public async Task<BreederBrand> Create_BreederBrand(string userId)
        {
            var user = await _accountServices.Get_UserById(userId);
            if (user == null) throw new Exception("User not found");

            var breederBrand = new BreederBrand
            {
                UserId = user.Id,
                BreederBrandOwner = user,
                BreederBrandName = $"{user.LastName}'s kaninavl",
                BreederBrandDescription = null,
                BreederBrandLogo = null
            };

            await _dbRepository.AddObjectAsync(breederBrand);
            return breederBrand;
        }

        public async Task<BreederBrand> Get_BreederBrandById(int id)
        {
            return await _dbRepository.GetObject_ByIntKEYAsync(id);
        }

        public async Task<BreederBrand> Get_BreederBrandByUserId(string userId)
        {
            return await _dbRepository.GetObject_ByFilterAsync(bb => bb.UserId == userId);
        }

        public async Task Update_BreederBrand(BreederBrand breederBrand)
        {
            await _dbRepository.UpdateObjectAsync(breederBrand);
        }

        public async Task Delete_BreederBrand(int id)
        {
            var breederBrand = await _dbRepository.GetObject_ByIntKEYAsync(id);
            if (breederBrand != null)
            {
                await _dbRepository.DeleteObjectAsync(breederBrand);
            }
        }
    }
}
