using DB_AngoraLib.Models;
using DB_AngoraLib.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Services.RabbitService
{
    public class RabbitService
    {
        private readonly IGRepository<Rabbit> _rabbitRepository;

        public RabbitService(IGRepository<Rabbit> rabbitRepository)
        {
            _rabbitRepository = rabbitRepository;
        }

        public async Task<IEnumerable<Rabbit>> GetRabbitsByUserIdAsync(string userId)
        {
            var rabbits = await _rabbitRepository.GetObjectsAsync();
            return rabbits.Where(rabbit => rabbit.Owner == userId);
        }

        public async Task AddRabbitAsync(Rabbit rabbit)
        {
            await _rabbitRepository.AddObjectAsync(rabbit);
        }

        public async Task UpdateRabbitAsync(Rabbit rabbit)
        {
            await _rabbitRepository.UpdateObjectAsync(rabbit);
        }

        public async Task DeleteRabbitAsync(int rabbitId)
        {
            var rabbit = await _rabbitRepository.GetObjectByIdAsync(rabbitId);
            if (rabbit != null)
            {
                await _rabbitRepository.DeleteObjectAsync(rabbit);
            }
        }

        public async Task SaveRabbitsAsync(List<Rabbit> rabbits)
        {
            await _rabbitRepository.SaveObjects(rabbits);
        }
    }
}
