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
                await Send_EmailConfirm_ToUser(newUser.Id, token);


                //// Trigger UserRegisteredEvent
                //var userRegisteredEvent = new UserRegistered_Event { Email = newUser.UserName };
                //await _eventBus.Trigger(userRegisteredEvent);
            }

            return responseDTO;
        }

        //---------------------------------: GET USER METHODS :---------------------------------
        //------------: GET ALL USERS
        public async Task<List<User>> GetAll_Users()
        {
            return (await _dbRepository.GetAllObjectsAsync()).ToList();
        }

        //------------: GET USER BY USERNAME OR EMAIL
        public async Task<User> Get_UserByUserNameOrEmail(string userNameOrEmail)
        {
            return await _dbRepository.GetDbSet()
                .FirstOrDefaultAsync(u => u.UserName == userNameOrEmail || u.Email == userNameOrEmail);
        }

        //------------: GET USER BY ID
        public async Task<User?> Get_UserById(string userId)
        {
            return await _dbRepository.GetObject_ByStringKEYAsync(userId);
        }

        public async Task<User?> Get_UserById_IncludingCollections(string userId)
        {
            return await _dbRepository.GetDbSet()
                //.AsNoTracking()
                //.Include(u => u.RabbitsOwned)
                //.Include(u => u.RabbitsLinked)
                .Include(u => u.RabbitTransfers_Issued)
                    .ThenInclude(rt => rt.Status == TransferStatus.Pending)
                .Include(u => u.RabbitTransfers_Received)
                    .ThenInclude(rt => rt.Status == TransferStatus.Pending)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        //------------: GET USER BY BREEDER-REG-NO
        public async Task<User?> Get_UserByBreederRegNo(string breederRegNo)
        {
            return await _dbRepository.GetDbSet()
                .FirstOrDefaultAsync(u => u.BreederRegNo == breederRegNo);
        }

        //------------: GET USER PROFILE
        public async Task<User_ProfileDTO> Get_User_Profile(string userId, string userProfileId, IList<Claim> userClaims)
        {
            var user = await Get_UserById(userId);
            if (user == null)
            {
                return null;
            }

            var hasPermissionToGetAnyUser = userClaims.Any(
                c => c.Type == "User:Read" && c.Value == "Any");

            if (user.Id != userProfileId && !hasPermissionToGetAnyUser)
            {
                throw new UnauthorizedAccessException("You do not have permission to access this profile.");
            }

            // Map User entity to User_ProfileDTO
            var userProfile = new User_ProfileDTO
            {
                BreederRegNo = user.BreederRegNo,
                FirstName = user.FirstName,
                LastName = user.LastName,
                RoadNameAndNo = user.RoadNameAndNo,
                ZipCode = user.ZipCode,
                City = user.City,
                Email = user.Email,
                Phone = user.PhoneNumber
            };

            return userProfile;
        }



        //---------------------------------: GET USERs ICOLLECTION METHODS :--------------------
        //-----------: Rabbits

        public async Task<List<Rabbit_PreviewDTO>> GetAll_RabbitsOwned_Filtered(
           string userId, Rabbit_FilteredRequestDTO filter)
        {

            var query = _dbRepository.GetDbSet()
                .AsNoTracking()
                .Include(u => u.RabbitsOwned)
                    .ThenInclude(rabbit => rabbit.UserOwner) // Inkluder UserOwner relationen
                .Include(u => u.RabbitsOwned)
                    .ThenInclude(rabbit => rabbit.UserOrigin) // Inkluder UserOrigin relationen
                .Where(u => u.Id == userId)
                .SelectMany(u => u.RabbitsOwned)
                .AsQueryable();



            // Filtrer baseret på død/levende status
            if (filter.OnlyDeceased.HasValue)
            {
                if (filter.OnlyDeceased.Value)
                {
                    // Vis kun døde kaniner
                    query = query.Where(r => r.DateOfDeath != null);
                }
                else
                {
                    // Vis kun levende kaniner
                    query = query.Where(r => r.DateOfDeath == null);
                }
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

            if (filter.ApprovedRaceColorCombination.HasValue)
                query = query.Where(r => r.ApprovedRaceColorCombination == filter.ApprovedRaceColorCombination.Value);
            
            if (filter.ForSale.HasValue)
                query = query.Where(r => r.ForSale == filter.ForSale.Value);
            if (filter.ForBreeding.HasValue)
                query = query.Where(r => r.ForBreeding == filter.ForBreeding.Value);

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

        public async Task<List<Rabbit_PreviewDTO>> GetAll_Rabbits_FromMyFold(string userId)
        {
            var userWithRabbitsLinked = await _dbRepository.GetDbSet()
                .AsNoTracking()
                .Include(u => u.RabbitsLinked)
                    .ThenInclude(rabbit => rabbit.UserOwner)
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

        //-----------: TransferRequests
        public async Task<List<TransferRequest_ReceivedDTO>> GetAll_TransferRequests_Received(
            string userId, TransferRequest_ReceivedFilterDTO filter)
        {
            var query = _dbRepository.GetDbSet()
                .AsNoTracking()
                .Include(u => u.RabbitTransfers_Received)
                    .ThenInclude(transfer => transfer.UserIssuer)
                .Include(u => u.RabbitTransfers_Received)
                    .ThenInclude(transfer => transfer.Rabbit)
                .Where(u => u.Id == userId)
                .SelectMany(u => u.RabbitTransfers_Received)
                .AsQueryable();

            if (filter.Status.HasValue)
            {
                query = query.Where(transfer => transfer.Status == filter.Status.Value);
            }

            if (filter.Rabbit_EarCombId != null)
                query = query.Where(t => EF.Functions.Like(t.RabbitId, $"%{filter.Rabbit_EarCombId}%"));

            if (filter.Rabbit_NickName != null)
                query = query.Where(t => EF.Functions.Like(t.Rabbit.NickName, $"%{filter.Rabbit_NickName}%"));

            if (filter.Issuer_BreederRegNo != null)
                query = query.Where(t => EF.Functions.Like(t.UserIssuer.BreederRegNo, $"%{filter.Issuer_BreederRegNo}%"));

            if (filter.Issuer_FirstName != null)
                query = query.Where(t => EF.Functions.Like(t.UserIssuer.FirstName, $"%{filter.Issuer_FirstName}%"));

            if (filter.From_DateAccepted.HasValue)
            {
                query = query.Where(transfer => transfer.DateAccepted >= filter.From_DateAccepted.Value);
            }

            var transferRequests = await query.Select(transfer => new TransferRequest_ReceivedDTO
            {
                Id = transfer.Id,
                Status = transfer.Status,
                DateAccepted = transfer.DateAccepted,
                Rabbit_EarCombId = transfer.Rabbit.EarCombId,
                Rabbit_NickName = transfer.Rabbit.NickName,
                Issuer_BreederRegNo = transfer.UserIssuer.BreederRegNo,
                Issuer_FirstName = transfer.UserIssuer.FirstName,
                Price = transfer.Price,
                SaleConditions = transfer.SaleConditions,
            }).ToListAsync();

            return transferRequests;
        }

        public async Task<List<TransferRequest_SentDTO>> GetAll_TransferRequests_Sent(
            string userId, TransferRequest_SentFilterDTO filter)
        {
            var query = _dbRepository.GetDbSet()
                .AsNoTracking()
                .Include(u => u.RabbitTransfers_Issued)
                    .ThenInclude(transfer => transfer.UserIssuer)
                .Include(u => u.RabbitTransfers_Issued)
                    .ThenInclude(transfer => transfer.Rabbit)
                .Where(u => u.Id == userId)
                .SelectMany(u => u.RabbitTransfers_Issued)
                .AsQueryable();


            if (filter.Status.HasValue)
            {
                query = query.Where(transfer => transfer.Status == filter.Status.Value);
            }

            if (filter.Rabbit_EarCombId != null)
                query = query.Where(t => EF.Functions.Like(t.RabbitId, $"%{filter.Rabbit_EarCombId}%"));

            if (filter.Rabbit_NickName != null)
                query = query.Where(t => EF.Functions.Like(t.Rabbit.NickName, $"%{filter.Rabbit_NickName}%"));

            if (filter.Recipent_BreederRegNo != null)
                query = query.Where(t => EF.Functions.Like(t.UserRecipent.BreederRegNo, $"%{filter.Recipent_BreederRegNo}%"));

            if (filter.Recipent_FirstName != null)
                query = query.Where(t => EF.Functions.Like(t.UserRecipent.FirstName, $"%{filter.Recipent_FirstName}%"));

            if (filter.From_DateAccepted.HasValue)
            {
                query = query.Where(transfer => transfer.DateAccepted >= filter.From_DateAccepted.Value);
            }

            // Konverter til TransferRequest_SentDTO
            var result = await query.Select(t => new TransferRequest_SentDTO
            {
                Id = t.Id,
                Status = t.Status,
                DateAccepted = t.DateAccepted,
                Rabbit_EarCombId = t.Rabbit.EarCombId,
                Rabbit_NickName = t.Rabbit.NickName,
                Recipent_BreederRegNo = t.UserRecipent.BreederRegNo,
                Recipent_FirstName = t.UserRecipent.FirstName,
                Price = t.Price,
                SaleConditions = t.SaleConditions
            }).ToListAsync();

            return result;
        }

        //-----------: ApplicationBreeder
        public async Task<List<ApplicationBreeder_PreviewDTO>> GetAll_ApplicationBreeder(string userId)
        {
            var user = await _dbRepository.GetDbSet()
                .AsNoTracking()
                .Include(u => u.BreederApplications)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                Console.WriteLine("Bruger ikke fundet");
                return new List<ApplicationBreeder_PreviewDTO>();
            }

            if (user.BreederApplications.Count < 1)
            {
                Console.WriteLine("Ingen ansøgninger for blive Breeder eksistere");
                return new List<ApplicationBreeder_PreviewDTO>();
            }

            return user.BreederApplications
                .Select(applicationBreeder => new ApplicationBreeder_PreviewDTO
                {
                    Id = applicationBreeder.Id,
                    Status = applicationBreeder.Status,
                    DateSubmitted = applicationBreeder.DateSubmitted,
                    UserApplicant_FullName = $"{applicationBreeder.UserApplicant.FirstName} {applicationBreeder.UserApplicant.LastName}",
                    UserApplicant_RequestedBreederRegNo = applicationBreeder.RequestedBreederRegNo,
                    UserApplicant_DocumentationPath = applicationBreeder.DocumentationPath,
                    RejectionReason = applicationBreeder.RejectionReason,                   
                })
                .ToList();
        }

        //public async Task<List<object>> GetAll_FavoriteItems(string userId)
        //{
        //    var userWithFavorites = await _dbRepository.GetDbSet()
        //        .AsNoTracking()
        //        .Include(u => u.Favorites)
        //        .FirstOrDefaultAsync(u => u.Id == userId);

        //    if (userWithFavorites == null)
        //    {
        //        Console.WriteLine("User not found");
        //        return new List<object>();
        //    }

        //    var favoriteItems = new List<object>();

        //    foreach (var favorite in userWithFavorites.Favorites)
        //    {
        //        if (favorite.ItemType == FavoriteType.Rabbit)
        //        {
        //            var rabbit = await _rabbitRepository.GetObject_ByStringKEYAsync(favorite.ItemId);
        //            if (rabbit != null)
        //            {
        //                favoriteItems.Add(new Rabbit_PreviewDTO
        //                {
        //                    EarCombId = rabbit.EarCombId,
        //                    NickName = rabbit.NickName,
        //                    Race = rabbit.Race,
        //                    Color = rabbit.Color,
        //                    Gender = rabbit.Gender,
        //                    UserOwner = rabbit.UserOwner?.UserName,
        //                    UserOrigin = rabbit.UserOrigin?.UserName
        //                });
        //            }
        //        }
        //        else if (favorite.ItemType == FavoriteType.Wool)
        //        {
        //            var wool = await _woolRepository.GetObject_ByStringKEYAsync(favorite.ItemId);
        //            if (wool != null)
        //            {
        //                favoriteItems.Add(new WoolDTO
        //                {
        //                    WoolType = wool.WoolType,
        //                    WoolColor = wool.WoolColor,
        //                    WoolQuality = wool.WoolQuality,
        //                    WoolLength = wool.WoolLength,
        //                    WoolWeight = wool.WoolWeight,
        //                    WoolDescription = wool.WoolDescription
        //                });
        //            }
        //        }
        //    }

        //    return favoriteItems;
        //}


        //---------------------------------: EMAIL METHODs :-------------------------------

        /// <summary>
        /// Modtager User.Id og et givent token, via Register metoden.
        /// Metoden her sender da en e-mail til brugeren, med et link til som brugeren kan bekræfte
        /// via et link i e-mailen, som benytter tokenet.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task Send_EmailConfirm_ToUser(string userId, string token)
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


        public async Task Send_PWResetRequest(string userEmail)
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
        public async Task Formular_ResetPW(string userId, string token, string newPassword)
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
        public async Task<IdentityResult> User_ChangePassword(User_ChangePasswordDTO userPwConfig)
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
