using DB_AngoraLib.DTOs;
using DB_AngoraLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Services.RabbitService
{
    public interface IRabbitService
    {
        Task<List<Rabbit>> GetAllRabbitsAsync();
        Task<List<Rabbit>> GetAllRabbits_ByBreederRegAsync(string breederRegNo);
        Task<Rabbit> GetRabbitByEarTagsAsync(string rightEarId, string leftEarId);
        Task<Rabbit> AddRabbit_ToMyCollectionAsync(string userId, RabbitDTO newRabbit);
        Task UpdateMyRabbitAsync(User currentUser, Rabbit rabbitId);
        Task UpdateRabbit_RBAC_Async(User currentUser, string rightEarId, string leftEarId, Rabbit_UpdateDTO updatedRabbit, IList<Claim> userClaims);
        //Task DeleteMyRabbitAsync(User currentUser, Rabbit rabbitToDelete);
        Task DeleteRabbit_RBAC_Async(User currentUser, string rightEarId, string leftEarId, IList<Claim> userClaims);
    }
}
