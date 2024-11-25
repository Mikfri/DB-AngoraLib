using DB_AngoraLib.Models;
using DB_AngoraLib.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Services.TokenService
{
    public class TokenServices : ITokenService
    {
        private readonly IGRepository<RefreshToken> _dbRepository;
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public TokenServices(IGRepository<RefreshToken> dbRepository, IConfiguration configuration, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _dbRepository = dbRepository;
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SaveUserTokenAsync(string userId, string loginProvider, string tokenName, string tokenValue)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException("Brugeren blev ikke fundet.");
            }

            var result = await _userManager.SetAuthenticationTokenAsync(user, loginProvider, tokenName, tokenValue);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException("Kunne ikke gemme tokenet.");
            }
        }


        public async Task<string> GenerateAccessToken(User user)
        {
            var keyString = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claimsList = new List<Claim>
            {
                // Tilføjer bruger ID claim
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };
            
            var userRoles = await _userManager.GetRolesAsync(user);
            // Tilføjer brugerens roller som claims
            foreach (var role in userRoles)
            {
                claimsList.Add(new Claim(ClaimTypes.Role, role));
            }

            //Tilføjer RoleClaims til token claims
            foreach (var roleName in userRoles)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                var roleClaims = await _roleManager.GetClaimsAsync(role);
                claimsList.AddRange(roleClaims);
            }

            // Tilføjer UserClaims til token claims
            var userClaims = await _userManager.GetClaimsAsync(user);
            claimsList.AddRange(userClaims);

            var now = DateTime.UtcNow;

            // Vi kan enten bruge SecurityTokenDescriptor som før
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claimsList),
                Expires = now.AddHours(1),
                IssuedAt = now,
                NotBefore = now,
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = creds
            };

            var tokenHandler = new JsonWebTokenHandler();
            return tokenHandler.CreateToken(tokenDescriptor);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public async Task UpdateRefreshTokenForUser(User user, string newRefreshToken, string createdByIp)
        {
            var refreshToken = new RefreshToken
            {
                Token = newRefreshToken,
                // Expires sættes automatisk i RefreshToken-konstruktøren eller metoden, der bruger TokenDuration
                Created = DateTime.UtcNow,
                CreatedByIp = createdByIp,
            };

            user.RefreshTokens.Add(refreshToken);
            await _userManager.UpdateAsync(user);
        }

        public async Task<User> GetUserByRefreshToken(string refreshToken)
        {
            var user = await _userManager.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == refreshToken && t.IsActive));

            return user;
        }

        public bool IsRefreshTokenValid(User user, string refreshToken)
        {
            return user.RefreshTokens.Any(t => t.Token == refreshToken && t.IsActive);
        }
    }
}

