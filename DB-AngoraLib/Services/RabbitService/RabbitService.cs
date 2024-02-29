using DB_AngoraLib.Models;
using DB_AngoraLib.Repository;
using DB_AngoraLib.Services.ValidationService;


namespace DB_AngoraLib.Services.RabbitService
{
    using IRabbitRepository = IGRepository<Rabbit>;
    using RabbitList = List<Rabbit>;

    public class RabbitService : IRabbitService
    {
        private readonly IRabbitRepository _dbRepository;
        private readonly ValidatorService _validatorService;
        //private readonly RabbitList _rabbitList = new List<Rabbit>(); // THIS MUST GO!


        public RabbitService(IGRepository<Rabbit> rabbitRepository, ValidatorService validatorService)
        {
            _dbRepository = rabbitRepository;
            _validatorService = validatorService;

            _rabbitList.Add(new Rabbit
            {
                Id = 1,
                Owner = "Ida.Fri@Gmail.com",
                RightEarId = "5095",
                LeftEarId = "002",
                Race = Race.Angora,
                Color = Color.Blå,
                ApprovedRaceColorCombination = null,
                DateOfBirth = new DateOnly(2019, 06, 01), // Format: yyyy-MM-dd
                Gender = Gender.Hun
            });

            _rabbitList.Add(new Rabbit
            {
                Id = 2,
                Owner = "Maja.Hulstrøm@Gmail.com",
                RightEarId = "5053",
                LeftEarId = "105",
                Race = Race.Satinangora,
                Color = Color.Jerngrå,
                ApprovedRaceColorCombination = null,
                DateOfBirth = new DateOnly(2020, 03, 15),
                Gender = Gender.Han
            });

            _rabbitList.Add(new Rabbit
            {
                Id = 3,
                Owner = "Ida.Fri@Gmail.com",
                RightEarId = "5053",
                LeftEarId = "011",
                Race = Race.Satinangora,
                Color = Color.Brun_Havana,
                ApprovedRaceColorCombination = null,
                DateOfBirth = new DateOnly(2018, 11, 28),
                Gender = Gender.Hun
            });

        }

        //---------------------: GET METODER
        public async Task<List<Rabbit>> GetAllRabbitsAsync()
        {
            var rabbits = await _dbRepository.GetObjectsAsync();
            return rabbits.ToList();
        }

        public async Task<Rabbit> GetRabbitByIdAsync(int id)
        {
            return await _dbRepository.GetObjectByIdAsync(id);
        }

        public async Task<List<Rabbit>> GetAllRabbitsByOwnerAsync(string userId)
        {
            var rabbits = await _dbRepository.GetObjectsAsync();
            return rabbits.Where(rabbit => rabbit.Owner == userId).ToList();
        }

        //---------------------: ADD
        public async Task AddRabbitAsync(Rabbit rabbit)
        {
            _validatorService.ValidateRabbit(rabbit);   // svarer her til ModelState.IsValid
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
