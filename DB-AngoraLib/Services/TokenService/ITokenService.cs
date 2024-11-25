using DB_AngoraLib.Models;
using Microsoft.IdentityModel.JsonWebTokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Services.TokenService
{
    public interface ITokenService
    {
        Task SaveUserTokenAsync(string userId, string loginProvider, string tokenName, string tokenValue);
        Task<string> GenerateAccessToken(User user);
        string GenerateRefreshToken();
        Task UpdateRefreshTokenForUser(User user, string newRefreshToken, string createdByIp);
        Task<User> GetUserByRefreshToken(string refreshToken);
        bool IsRefreshTokenValid(User user, string refreshToken);
    }
}
