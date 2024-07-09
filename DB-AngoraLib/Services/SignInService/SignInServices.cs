using DB_AngoraLib.DTOs;
using DB_AngoraLib.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Services.SigninService
{
    public class SigninServices : ISigninService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;

        public SigninServices(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, SignInManager<User> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        //---------------------------------------: LOGIN :---------------------------------------
        
        public async Task<Login_ResponseDTO> LoginAsync(Login_RequestDTO loginRequestDTO)
        {
            var result = await _signInManager.PasswordSignInAsync(
                loginRequestDTO.UserName,
                loginRequestDTO.Password,
                loginRequestDTO.RememberMe,
                lockoutOnFailure: false);

            var loginResponseDTO = new Login_ResponseDTO();

            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(loginRequestDTO.UserName);

                // Generate the token with the user's claims
                var token = await GenerateToken(user);
                var tokenHandler = new JwtSecurityTokenHandler();

                loginResponseDTO.UserName = user.UserName;
                loginResponseDTO.Token = tokenHandler.WriteToken(token);
                loginResponseDTO.ExpiryDate = token.ValidTo;
            }
            else
            {
                loginResponseDTO.Errors.Add("Invalid login attempt.");
            }

            return loginResponseDTO;
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }


        //---------------------------------------: TOKEN SETUP :---------------------------------------


        private async Task<JwtSecurityToken> GenerateToken(User user)
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

        public async Task<Token_ResponseDTO> RefreshTokenAsync(string refreshToken)
        {
            var user = await GetUserByRefreshToken(refreshToken);
            if (user == null || !IsRefreshTokenValid(user, refreshToken))
            {
                return null; // Eller en fejlmeddelelse
            }

            var newAccessToken = await GenerateToken(user);
            var newRefreshToken = GenerateRefreshToken();

            // Opdater brugerens refresh token i databasen
            await UpdateRefreshTokenForUser(user, newRefreshToken);

            return new Token_ResponseDTO
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                RefreshToken = newRefreshToken
            };
        }

        private string GenerateRefreshToken()
        {
            // Generer en tilfældig streng som refresh token
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        // Antag at denne metode opdaterer brugerens refresh token i databasen
        private async Task UpdateRefreshTokenForUser(User user, string newRefreshToken)
        {
            var refreshToken = new RefreshToken
            {
                Token = newRefreshToken,
                Expires = DateTime.UtcNow.AddDays(7), // Sæt en udløbsdato, f.eks. 7 dage fra nu
                Created = DateTime.UtcNow,
                CreatedByIp = "" // Hvis du har brugerens IP, kan du sætte den her
            };

            user.RefreshTokens.Add(refreshToken);
            await _userManager.UpdateAsync(user);
        }


        // Antag at denne metode finder en bruger baseret på et refresh token
        private async Task<User> GetUserByRefreshToken(string refreshToken)
        {
            var user = await _userManager.Users
                .Include(u => u.RefreshTokens) // Sørg for at inkludere RefreshTokens i din query
                .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == refreshToken && t.IsActive));

            return user;
        }


        // Validerer om et refresh token er gyldigt (ikke udløbet og tilhører brugeren)
        private bool IsRefreshTokenValid(User user, string refreshToken)
        {
            return user.RefreshTokens.Any(t => t.Token == refreshToken && t.IsActive);
        }




        //---------------------------------------: EXTERNAL LOGIN :---------------------------------------

        public async Task<SignInResult> ExternalLoginSignInAsync(string loginProvider, string providerKey, bool isPersistent)
        {
            return await _signInManager.ExternalLoginSignInAsync(loginProvider, providerKey, isPersistent, bypassTwoFactor: true);
        }

        public async Task<string> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                throw new Exception("Error loading external login information during confirmation.");
            }

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                return RedirectToLocal(returnUrl);
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new User { UserName = email, Email = email };
                await _userManager.CreateAsync(user);
            }

            await _userManager.AddLoginAsync(user, info);
            await _signInManager.SignInAsync(user, isPersistent: false);

            return RedirectToLocal(returnUrl);
        }

        private bool IsLocalUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return false;
            }

            return Uri.TryCreate(url, UriKind.Relative, out Uri result);
        }

        private string RedirectToLocal(string returnUrl)
        {
            if (IsLocalUrl(returnUrl))
            {
                return returnUrl;
            }
            else
            {
                return "/";
            }
        }
    }
}