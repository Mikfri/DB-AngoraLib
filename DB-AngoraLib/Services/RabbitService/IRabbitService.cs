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
        Task<Rabbit> GetRabbitByEarTagsAsync(string rightEarId, string leftEarId);
        List<Rabbit> GetRabbitsByProperties(User currentUser, Race race, Color color, Gender gender, IsPublic isPublic, string rightEarId, string leftEarId);
        Task AddRabbit_ToCurrentUserAsync(User currentUser, Rabbit newRabbit);   
        Task UpdateRabbitAsync(Rabbit rabbitId, User thisUser);
        Task DeleteRabbitAsync(Rabbit rabbitToDelete, User thisUser);
    }
}
