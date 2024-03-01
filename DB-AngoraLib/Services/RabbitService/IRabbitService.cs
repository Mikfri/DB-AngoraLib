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
        Task<List<Rabbit>> GetAllRabbitsAsync();
        Task<Rabbit> GetRabbitByIdAsync(int rabbitId);
        Task<Rabbit> GetRabbitByEarIdAsync(string rightEarId, string leftEarId);
        Task<List<Rabbit>> GetAllRabbitsByOwnerAsync(int userId);
        Task AddRabbitAsync(Rabbit newRabbit, User userId);
        Task UpdateRabbitAsync(Rabbit rabbitId, User userId);
        Task DeleteRabbitAsync(int rabbitId, User userId);
    }
}
