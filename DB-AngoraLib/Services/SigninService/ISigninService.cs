using DB_AngoraLib.DTOs;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Services.SigninService
{
    public interface ISigninService
    {
        Task<Login_ResponseDTO> LoginAsync(string userIP, Login_RequestDTO loginRequest);
        Task LogoutAsync();
        //Task<ExternalLoginInfo> GetExternalLoginInfoAsync();
        Task<string> ExternalLoginCallback(string returnUrl = null, string remoteError = null);
    }
}