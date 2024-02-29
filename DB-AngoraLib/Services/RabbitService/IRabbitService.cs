using DB_AngoraLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Services.RabbitService
{
    public interface IRabbitService
    {
        Task<Rabbit> GetRabbitByIdAsync(int id);
        Task<List<Rabbit>> GetAllRabbitsAsync();
        Task<List<Rabbit>> GetAllRabbitsByOwnerAsync(string userId);
        Task AddRabbitAsync(Rabbit rabbit, User user);
        Task UpdateRabbitAsync(Rabbit rabbit);
        Task DeleteRabbitAsync(int Id);
    }
}
