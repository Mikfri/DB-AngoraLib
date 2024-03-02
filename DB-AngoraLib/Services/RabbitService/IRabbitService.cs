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
        Task<Rabbit> GetRabbitByEarTagsAsync(string rightEarId, string leftEarId);
        Task<List<Rabbit>> GetAllRabbitsByOwnerAsync(int userId);
        Task AddRabbitAsync(Rabbit newRabbit, User thisUser);
        Task UpdateRabbitAsync(Rabbit rabbitId, User thisUser);
        Task DeleteRabbitAsync(Rabbit rabbitToDelete, User thisUser);
    }
}
