using DB_AngoraLib.DTOs;
using DB_AngoraLib.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Services.RoleService
{
    public interface IRoleService
    {
        //Task AddSpecialPermissionToUser(string userId, string claimValue);
        Task<User_RolesAndClaimsDTO> GetUserRolesAndClaims(string userId);
        //Task RemoveSpecialPermissionFromUser(string userId, string claimValue);
    }
}
