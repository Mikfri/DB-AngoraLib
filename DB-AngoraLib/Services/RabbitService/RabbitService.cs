using DB_AngoraLib.EF_DbContext;
using DB_AngoraLib.Models;
using DB_AngoraLib.Repository;
using DB_AngoraLib.Services.UserService;
using DB_AngoraLib.Services.ValidationService;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


namespace DB_AngoraLib.Services.RabbitService
{
    //using IRabbitRepository = IGRepository<Rabbit>;
    //using RabbitList = List<Rabbit>;

    public class RabbitService : IRabbitService
    {
        private readonly IGRepository<Rabbit> _dbRepository;
        private readonly IUserService _userService;
        private readonly RabbitValidator _validatorService;


        public RabbitService(IGRepository<Rabbit> dbRepository, IUserService userService, RabbitValidator validatorService)
        {
            _dbRepository = dbRepository;
            _userService = userService;
            _validatorService = validatorService;
        }

        public RabbitService() { }

        //public void ValidateUniqueRabbitId(Rabbit rabbit)
        //{
        //    var existingRabbit = _dbRepository.GetObjectByIdAsync(Rabbit rabbit).Result;

        //    if (existingRabbit != null)
        //    {
        //        throw new InvalidOperationException($"A Rabbit with the id {rabbit.RightEarId}-{rabbit.LeftEarId} already exists.");
        //    }
        //}

        //---------------------: GET METODER
        //-------: ADMIN METODER

        public async Task<List<Rabbit>> GetAllRabbitsAsync()
        {
            var rabbits = await _dbRepository.GetAllObjectsAsync();
            return rabbits.ToList();
        }

        public async Task<List<Rabbit>> GetAllRabbits_ByBreederRegAsync(string breederRegNo)
        {
            var user = await _userService.GetUserByBreederRegNoAsync(breederRegNo);
            var rabbits = await _dbRepository.GetAllObjectsAsync();
            return rabbits.Where(rabbit => rabbit.OwnerId == user.BreederRegNo).ToList();
        }

        /// <summary>
        /// Denne metode behøver IKKE at være asynkron.
        /// Den opererer på data, der allerede er indlæst i hukommelsen 
        /// (dvs., currentUser.Rabbits), så der er ingen asynkrone operationer,
        /// der skal vente på. Derfor kan metoden returnere resultatet
        /// direkte som en List<Rabbit> i stedet for en Task<List<Rabbit>>.
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="race"></param>
        /// <param name="color"></param>
        /// <param name="gender"></param>
        /// <param name="isPublic"></param>
        /// <param name="rightEarId"></param>
        /// <param name="leftEarId"></param>
        /// <returns></returns>
        public List<Rabbit> GetRabbitsByProperties(User currentUser, string rightEarId, string leftEarId, string nickName, Race race, Color color, Gender gender, IsPublic isPublic, bool? isJuvenile, DateOnly? dateOfBirth, DateOnly? dateOfDeath)
        {
            return currentUser.Rabbits
                .Where(rabbit =>
                       rabbit.RightEarId == rightEarId
                    && rabbit.LeftEarId == leftEarId
                    && (nickName == null || rabbit.NickName == nickName)
                    && rabbit.Race == race
                    && rabbit.Color == color
                    && rabbit.Gender == gender
                    && rabbit.IsPublic == isPublic
                    && (isJuvenile == null || rabbit.IsJuvenile == isJuvenile)
                    && (dateOfBirth == null || rabbit.DateOfBirth == dateOfBirth)
                    && (dateOfDeath == null || rabbit.DateOfDeath == dateOfDeath))
                .ToList();
        }

        //-------: GET BY EAR TAGs METODER
        public async Task<Rabbit> GetRabbitByEarTagsAsync(string rightEarId, string leftEarId)
        {
            Expression<Func<Rabbit, bool>> filter = r => r.RightEarId == rightEarId && r.LeftEarId == leftEarId;

            Console.WriteLine($"Checking for existing Rabbit with ear tags: RightEarId: {rightEarId}, LeftEarId: {leftEarId}");

            var rabbit = await _dbRepository.GetObjectAsync(filter);

            if (rabbit != null)
            {
                Console.WriteLine($"A Rabbit with this ear-tag already exists!\nRabbitName: {rabbit.NickName}, OwnerId: {rabbit.OwnerId}, OwnerName: {rabbit.Owner.FirstName} {rabbit.Owner.LastName}");
            }
            else
            {
                Console.WriteLine("No rabbit found with the given ear tags.");
            }

            return rabbit;
        }

        

        //---------------------: ADD
        public async Task AddRabbit_ToCurrentUserAsync(User currentUser, Rabbit newRabbit)
        {
            Console.WriteLine($"Trying to add rabbit with RightEarId: {newRabbit.RightEarId}, LeftEarId: {newRabbit.LeftEarId}");
            _validatorService.ValidateRabbit(newRabbit);

            // Check om kaninen med de givne øremærker allerede eksisterer
            var existingRabbit = await GetRabbitByEarTagsAsync(newRabbit.RightEarId, newRabbit.LeftEarId);
            if (existingRabbit != null)
            {
                throw new InvalidOperationException("A rabbit with the same ear tags already exists.");
            }

            newRabbit.OwnerId = currentUser.BreederRegNo;
            await _dbRepository.AddObjectAsync(newRabbit);
            Console.WriteLine($"Rabbit added successfully with RightEarId: {newRabbit.RightEarId}, LeftEarId: {newRabbit.LeftEarId}, OwnerId: {newRabbit.OwnerId}");
        }
                

        //---------------------: UPDATE
        public async Task UpdateRabbitAsync(User currentUser, Rabbit rabbitId)
        {
            if (rabbitId.OwnerId != currentUser.BreederRegNo)
            {
                throw new InvalidOperationException("You are not the owner of this rabbit.");
            }
            _validatorService.ValidateRabbit(rabbitId);
            await _dbRepository.UpdateObjectAsync(rabbitId);
        }

        //---------------------: DELETE
        public async Task DeleteRabbitAsync(User currentUser, Rabbit rabbitToDelete)
        {
            if (rabbitToDelete.OwnerId != currentUser.BreederRegNo)
            {
                throw new InvalidOperationException("You are not the owner of this rabbit.");
            }

            await _dbRepository.DeleteObjectAsync(rabbitToDelete);
        }


    }
}
