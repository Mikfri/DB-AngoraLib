using DB_AngoraLib.Models;
using DB_AngoraLib.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
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

        public async Task<JwtSecurityToken> GenerateAccessToken(User user)
        {
            var keyString = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claimsList = new List<Claim>();

            // Tilføjer UserClaims til token claims
            var userClaims = await _userManager.GetClaimsAsync(user);
            claimsList.AddRange(userClaims);

            //Tilføjer RoleClaims til token claims
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var roleName in userRoles)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                var roleClaims = await _roleManager.GetClaimsAsync(role);
                claimsList.AddRange(roleClaims);
            }

            // Tilføjer brugerens roller som claims
            foreach (var role in userRoles)
            {
                claimsList.Add(new Claim(ClaimTypes.Role, role));
            }

            // Tilføj standard claims
            var iat = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            claimsList.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
            claimsList.Add(new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"));
            claimsList.Add(new Claim(ClaimTypes.Email, user.Email));
            claimsList.Add(new Claim(JwtRegisteredClaimNames.Iat, iat.ToString(), ClaimValueTypes.Integer64));

            var expires = DateTime.UtcNow.AddHours(1);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claimsList,
                notBefore: DateTime.UtcNow,
                expires: expires,
                signingCredentials: creds);

            return token;
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
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                CreatedByIp = createdByIp // Sæt brugerens IP-adresse her
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

