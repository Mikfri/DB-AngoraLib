using DB_AngoraLib.DTOs;
using DB_AngoraLib.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DB_AngoraLib.Services.AccountService
{
    public interface IAccountService
    {
        Task<Register_ResponseDTO> Register_BasicUserAsync(Register_CreateBasicUserDTO newUserDto);
        Task<List<User>> Get_AllUsersAsync();
        Task<User> Get_UserByUserNameOrEmailAsync(string userNameOrEmail);
        Task<User?> Get_UserByIdAsync(string userId);
        Task<User> Get_UserByBreederRegNoAsync(string breederRegNo);

        Task<List<Rabbit_PreviewDTO>> Get_MyRabbitCollection(string userId);
        Task<List<Rabbit_PreviewDTO>> Get_Rabbits_FromMyFold(string userId);
        Task<List<Rabbit_PreviewDTO>> Get_Rabbits_OwnedAlive_FilteredAsync(string userId, Rabbit_FilteredRequestDTO filter);
        Task<List<TransferRequest_ReceivedDTO>> Get_TransferRequests_Received(string userId, TransferRequest_ReceivedFilterDTO filter);

        Task Send_EmailConfirmAsync(string userId, string token);
        Task Formular_ResetPWAsync(string userId, string token, string newPassword);
        Task<IdentityResult> User_ChangePasswordAsync(User_ChangePasswordDTO userPwConfig);
      
    }
}
