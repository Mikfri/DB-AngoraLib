using DB_AngoraLib.Models;
using DB_AngoraLib.Repository;
using DB_AngoraLib.Services.ValidationService;


namespace DB_AngoraLib.Services.RabbitService
{
    //using IRabbitRepository = IGRepository<Rabbit>;
    //using RabbitList = List<Rabbit>;

    public class RabbitService : IRabbitService
    {
        private readonly IGRepository<Rabbit> _dbRepository;
        private readonly RabbitValidator _validatorService;


        public RabbitService(IGRepository<Rabbit> dbRepository, RabbitValidator validatorService)
        {
            _dbRepository = dbRepository;
            _validatorService = validatorService;
        }

        //---------------------: GET METODER
        public async Task<List<Rabbit>> GetAllRabbitsAsync()
        {
            var rabbits = await _dbRepository.GetAllObjectsAsync();
            return rabbits.ToList();
        }

        public async Task<Rabbit> GetRabbitByIdAsync(int id)
        {
            return await _dbRepository.GetObjectByIdAsync(id);
        }

        public async Task<List<Rabbit>> GetAllRabbitsByOwnerAsync(string userId)
        {
            var rabbits = await _dbRepository.GetAllObjectsAsync();
            return rabbits.Where(rabbit => rabbit.Owner == userId).ToList();
        }

        //---------------------: ADD
        public async Task AddRabbitAsync(Rabbit rabbit, User user)
        {
            _validatorService.ValidateRabbit(rabbit);
            user.AddRabbit(rabbit); // Brugerens metode til at tilføje kanin og opdatere ejerrelationen
            await _dbRepository.AddObjectAsync(rabbit);
        }

        //---------------------: UPDATE METODER
        public async Task UpdateRabbitAsync(Rabbit rabbit)
        {
            _validatorService.ValidateRabbit(rabbit);
            await _dbRepository.UpdateObjectAsync(rabbit);
        }

        //---------------------: DELETE
        public async Task DeleteRabbitAsync(int Id)
        {
            var rabbit = await _dbRepository.GetObjectByIdAsync(Id);
            if (rabbit != null)
            {
                await _dbRepository.DeleteObjectAsync(rabbit);
            }
        }

    }
}
