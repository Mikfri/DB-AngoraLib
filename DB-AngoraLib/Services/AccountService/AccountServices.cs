using DB_AngoraLib.DTOs;
using DB_AngoraLib.Models;
using Microsoft.AspNetCore.Identity;
using DB_AngoraLib.Services.EmailService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DB_AngoraLib.Services.HelperService;
using DB_AngoraLib.Repository;
using Microsoft.EntityFrameworkCore;
using DB_AngoraLib.Events;
using DB_AngoraLib.Services.ValidationService;
using Microsoft.Extensions.Caching.Memory;

namespace DB_AngoraLib.Services.AccountService
{
    public class AccountServices : IAccountService
    {
        private readonly IGRepository<User> _dbRepository;
        private readonly IEmailService _emailService;
        private readonly UserManager<User> _userManager;

        //private readonly IMemoryCache _cache;


        public AccountServices(IGRepository<User> dbRepository, IEmailService emailService, UserManager<User> userManager/*, IMemoryCache cache*/)
        {
            _dbRepository = dbRepository;
            _emailService = emailService;
            _userManager = userManager;

            //_cache = cache;
        }

        //---------------------------------: CREATE/REGISTER USER :---------------------------------
        public async Task<Register_ResponseDTO> Register_BasicUserAsync(Register_CreateBasicUserDTO newUserDTO)
        {
            var newUser = new User();
            newUserDTO.CopyPropertiesTo(newUser);
            newUser.UserName = newUserDTO.Email;

            var result = await _userManager.CreateAsync(newUser, newUserDTO.Password);
            // returnere IdentityResult OG UserName
            var responseDTO = new Register_ResponseDTO // <IdentityResult> alternativ!
            {
                UserName = newUser.UserName,
                IsSuccessful = result.Succeeded,
                Errors = result.Errors.Select(e => e.Description)
            };

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, "Guest"); // hardcoded role

                // Generer e-mail bekræftelsestoken
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);

                // Send bekræftelses e-mail
                await Send_EmailConfirmAsync(newUser.Id, token);


                //// Trigger UserRegisteredEvent
                //var userRegisteredEvent = new UserRegistered_Event { Email = newUser.UserName };
                //await _eventBus.Trigger(userRegisteredEvent);
            }

            return responseDTO;
        }

        //---------------------------------: GET USER METHODS :---------------------------------
        //------------: GET ALL USERS
        public async Task<List<User>> Get_AllUsersAsync()
        {
            return (await _dbRepository.GetAllObjectsAsync()).ToList();
        }

        //------------: GET USER BY USERNAME OR EMAIL
        public async Task<User> Get_UserByUserNameOrEmailAsync(string userNameOrEmail)
        {
            return await _dbRepository.GetDbSet()
                .FirstOrDefaultAsync(u => u.UserName == userNameOrEmail || u.Email == userNameOrEmail);
        }

        //------------: GET USER BY ID
        public async Task<User> Get_UserByIdAsync(string userId)
        {
            return await _dbRepository.GetObject_ByStringKEYAsync(userId);
        }

        //------------: GET USER BY BREEDER-REG-NO
        public async Task<User?> Get_UserByBreederRegNoAsync(string breederRegNo)
        {
            return await _dbRepository.GetDbSet()
                .FirstOrDefaultAsync(u => u.BreederRegNo == breederRegNo);
        }


        //---------------------------------: GET USERs ICOLLECTION METHODS :--------------------       
        public async Task<List<Rabbit_PreviewDTO>> Get_MyRabbitCollection(string userId)
        {
            var currentUserCollection = await _dbRepository.GetDbSet()
                .AsNoTracking()
                .Include(u => u.RabbitsOwned)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (currentUserCollection == null)
            {
                Console.WriteLine("User not found");
                return new List<Rabbit_PreviewDTO>();
            }

            if (currentUserCollection.RabbitsOwned.Count < 1)
            {
                Console.WriteLine("No rabbits found in collection");
                return new List<Rabbit_PreviewDTO>();
            }

            return currentUserCollection.RabbitsOwned
                .Select(rabbit => new Rabbit_PreviewDTO
                {
                    EarCombId = rabbit.EarCombId,
                    NickName = rabbit.NickName,
                    Race = rabbit.Race,
                    Color = rabbit.Color,
                    Gender = rabbit.Gender,
                    //UserOwner = rabbit.UserOwner != null ? $"{rabbit.UserOwner.FirstName} {rabbit.UserOwner.LastName}" : null,
                    UserOwner = rabbit.UserOwner?.FirstName + rabbit.UserOwner?.LastName,
                    //UserOrigin = rabbit.UserOrigin != null ? $"{rabbit.UserOrigin.FirstName} {rabbit.UserOrigin.LastName}" : null,
                    UserOrigin = rabbit.UserOrigin?.FirstName + rabbit.UserOrigin?.LastName,
                })
                .ToList();
        }

        public async Task<List<Rabbit_PreviewDTO>> Get_Rabbits_OwnedAlive_FilteredAsync(
           string userId, Rabbit_FilteredRequestDTO filter)
        {
            //var query = _dbRepository.GetDbSet()
            //    .AsNoTracking()
            //    .Include(u => u.RabbitsOwned)
            //    .Where(u => u.Id == userId)
            //    .SelectMany(u => u.RabbitsOwned)
            //    .AsQueryable();
            var query = _dbRepository.GetDbSet()
                .AsNoTracking()
                .Include(u => u.RabbitsOwned)
                    .ThenInclude(rabbit => rabbit.UserOwner) // Inkluder UserOwner relationen
                .Include(u => u.RabbitsOwned)
                    .ThenInclude(rabbit => rabbit.UserOrigin) // Inkluder UserOrigin relationen
                .Where(u => u.Id == userId)
                .SelectMany(u => u.RabbitsOwned)
                .AsQueryable();

            if (!filter.IncludeDeceased)
            {
                query = query.Where(r => r.DateOfDeath == null);
            }

            // Anvend filtrering baseret på Rabbit_FilteredRequestDTO
            if (filter.RightEarId != null)
                query = query.Where(r => EF.Functions.Like(r.RightEarId, $"%{filter.RightEarId}%"));
            if (filter.LeftEarId != null)
                query = query.Where(r => EF.Functions.Like(r.LeftEarId, $"%{filter.LeftEarId}%"));
            if (filter.NickName != null)
                query = query.Where(r => EF.Functions.Like(r.NickName, $"%{filter.NickName}%"));
            if (filter.Race.HasValue)
                query = query.Where(r => r.Race == filter.Race.Value);
            if (filter.Color.HasValue)
                query = query.Where(r => r.Color == filter.Color.Value);
            if (filter.Gender.HasValue)
                query = query.Where(r => r.Gender == filter.Gender.Value);
            if (filter.FromDateOfBirth.HasValue)
                query = query.Where(r => r.DateOfBirth > filter.FromDateOfBirth.Value);
            if (filter.FromDateOfDeath.HasValue)
                query = query.Where(r => r.DateOfDeath.HasValue && r.DateOfDeath > filter.FromDateOfDeath.Value);

            // Håndter IsJuvenile
            if (filter.IsJuvenile.HasValue)
            {
                var today = DateOnly.FromDateTime(DateTime.Now);
                var juvenileStart = today.AddDays(-14 * 7); // Starten af juvenile alderen (14 uger tilbage)
                var juvenileEnd = today.AddDays(-8 * 7); // Slutningen af juvenile alderen (8 uger tilbage)

                if (filter.IsJuvenile.Value)
                {
                    query = query.Where(r => r.DateOfBirth >= juvenileStart && r.DateOfBirth <= juvenileEnd);
                }
                else
                {
                    query = query.Where(r => r.DateOfBirth < juvenileStart || r.DateOfBirth > juvenileEnd);
                }
            }

            // Filtrering baseret på FatherId_Placeholder og MotherId_Placeholder
            if (filter.FatherId_Placeholder != null)
                query = query.Where(r => EF.Functions.Like(r.FatherId_Placeholder, $"%{filter.FatherId_Placeholder}%"));
            if (filter.MotherId_Placeholder != null)
                query = query.Where(r => EF.Functions.Like(r.MotherId_Placeholder, $"%{filter.MotherId_Placeholder}%"));

            var queryRabbitsList = await query.ToListAsync();

            var rabbitPreviewDTOsList = queryRabbitsList.Select(rabbit => new Rabbit_PreviewDTO
            {
                EarCombId = rabbit.EarCombId,
                NickName = rabbit.NickName,
                Race = rabbit.Race,
                Color = rabbit.Color,
                Gender = rabbit.Gender,
                UserOwner = rabbit.UserOwner != null ? $"{rabbit.UserOwner.FirstName} {rabbit.UserOwner.LastName}" : null,
                UserOrigin = rabbit.UserOrigin != null ? $"{rabbit.UserOrigin.FirstName} {rabbit.UserOrigin.LastName}" : null,
            }).ToList();

            return rabbitPreviewDTOsList;
        }


        public async Task<List<Rabbit_PreviewDTO>> Get_Rabbits_FromMyFold(string userId)
        {
            var userWithRabbitsLinked = await _dbRepository.GetDbSet()
                .AsNoTracking()
                .Include(u => u.RabbitsLinked)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (userWithRabbitsLinked == null)
            {
                Console.WriteLine("User not found");
                return new List<Rabbit_PreviewDTO>();
            }

            if (userWithRabbitsLinked.RabbitsLinked.Count < 1)
            {
                Console.WriteLine("No linked rabbits found in collection");
                return new List<Rabbit_PreviewDTO>();
            }

            return userWithRabbitsLinked.RabbitsLinked
                .Select(rabbit => new Rabbit_PreviewDTO
                {
                    EarCombId = rabbit.EarCombId,
                    NickName = rabbit.NickName,
                    Race = rabbit.Race,
                    Color = rabbit.Color,
                    Gender = rabbit.Gender,
                    UserOwner = rabbit.UserOwner != null ? $"{rabbit.UserOwner.FirstName} {rabbit.UserOwner.LastName}" : null,
                    UserOrigin = rabbit.UserOrigin != null ? $"{rabbit.UserOrigin.FirstName} {rabbit.UserOrigin.LastName}" : null,
                })
                .ToList();
        }


        //---------------------------------: EMAIL METHODs :-------------------------------

        public async Task Send_EmailConfirmAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId) 
                ?? throw new Exception("User not found");
            


            // Antag at du har en frontend-rute setup til at håndtere e-mail bekræftelse
            var confirmationLink = $"https://DB-Angora.dk/email-confirmation?userId={HttpUtility.UrlEncode(userId)}&token={HttpUtility.UrlEncode(token)}";

            // Konstruer e-mail beskeden
            var emailSubject = "DB-Angora: Velkommen til";
            var emailBody = $"Bekræft venligst din e-mail ved at klikke på følgende link, for at fuldende din profil: <a href=\"{confirmationLink}\">Bekræft e-mail</a>";

            // Send bekræftelses e-mail
            await _emailService.SendEmailAsync(user.Email, emailSubject, emailBody);
        }


        public async Task Send_PWResetRequestAsync(string userEmail)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            // Generer nulstillingstoken
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Konstruer URL'en til nulstilling af adgangskoden
            var resetLink = $"https://DB-Angora.dk/password-reset?userId={HttpUtility.UrlEncode(user.Id)}&token={HttpUtility.UrlEncode(resetToken)}";

            // Konstruer e-mail beskeden
            var emailSubject = "DB-Angora: Nulstil din adgangskode";
            var emailBody = $"For at nulstille din adgangskode, venligst klik på følgende link: <a href=\"{resetLink}\">Nulstil adgangskode</a>";

            // Send e-mailen
            await _emailService.SendEmailAsync(user.Email, emailSubject, emailBody);
        }



        //---------------: RESET PASSWORD FORMULAR

        /// <summary>
        /// Formular til brugeren på frontend, hvor brugeren indtaster ny password.
        /// Metoden kommer i brug efter metoden: Send_PWResetRequestAsync
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task Formular_ResetPWAsync(string userId, string token, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (!result.Succeeded)
            {
                throw new Exception("Password reset failed");
            }
        }


        //---------------------------------: UPDATE ACCOUNT Settings :-----------------------------------
        public async Task<IdentityResult> User_ChangePasswordAsync(User_ChangePasswordDTO userPwConfig)
        {
            var user = await _userManager.FindByIdAsync(userPwConfig.UserId);
            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }

            return await _userManager.ChangePasswordAsync(user, userPwConfig.CurrentPassword, userPwConfig.NewPassword);
        }



       

    }
}
