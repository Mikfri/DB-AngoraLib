using DB_AngoraLib.DTOs;
using DB_AngoraLib.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DB_AngoraLib.Services.AccountService
{
    public interface IAccountService
    {
        Task<Register_ResponseDTO> Register_BasicUserAsync(Register_CreateBasicUserDTO newUserDto);
        Task<User> Get_UserByUserNameOrEmail(string userNameOrEmail);
        Task<User?> Get_UserById(string userId);
        Task<User> Get_UserByBreederRegNo(string breederRegNo);
        Task<List<User>> GetAll_Users();

        Task<User_ProfileDTO> Get_User_Profile(string userId, string userProfileId, IList<Claim> userClaims);
        Task<List<Rabbit_PreviewDTO>> GetAll_Rabbits_FromMyFold(string userId);
        Task<List<Rabbit_PreviewDTO>> GetAll_RabbitsOwned_Filtered(string userId, Rabbit_FilteredRequestDTO filter);
        Task<List<TransferRequest_ReceivedDTO>> GetAll_TransferRequests_Received(string userId, TransferRequest_ReceivedFilterDTO filter);
        Task<List<TransferRequest_SentDTO>> GetAll_TransferRequests_Sent(string userId, TransferRequest_SentFilterDTO filter);
        Task<List<ApplicationBreeder_PreviewDTO>> GetAll_ApplicationBreeder(string userId);


        Task Send_EmailConfirm_ToUser(string userId, string token);
        Task Formular_ResetPW(string userId, string token, string newPassword);
        Task<IdentityResult> User_ChangePassword(User_ChangePasswordDTO userPwConfig);      
    }
}
