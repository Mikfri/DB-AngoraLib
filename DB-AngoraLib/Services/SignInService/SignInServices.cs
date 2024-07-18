using DB_AngoraLib.DTOs;
using DB_AngoraLib.Models;
using DB_AngoraLib.Services.TokenService;
using Microsoft.AspNetCore.Http;
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
        private readonly ITokenService _tokenService;

        public SigninServices(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, SignInManager<User> signInManager,IConfiguration configuration, ITokenService tokenService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _tokenService = tokenService;
        }

        //---------------------------------------: LOGIN :---------------------------------------

        public async Task<Login_ResponseDTO> LoginAsync(string userIP, Login_RequestDTO loginRequestDTO)
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
                // Brug TokenServices til at generere tokens
                var accessToken = await _tokenService.GenerateAccessToken(user);
                var refreshToken = _tokenService.GenerateRefreshToken();

                // Opdater brugerens refresh token i databasen med den videregivne IP-adresse
                await _tokenService.UpdateRefreshTokenForUser(user, refreshToken, userIP);

                var tokenHandler = new JwtSecurityTokenHandler();
                var writtenAccessToken = tokenHandler.WriteToken(accessToken);

                loginResponseDTO.UserName = user.UserName;
                loginResponseDTO.AccessToken = writtenAccessToken;
                loginResponseDTO.ExpiryDate = accessToken.ValidTo;
                loginResponseDTO.RefreshToken = refreshToken;

                // Gem access token og refresh token ved hjælp af SaveUserTokenAsync
                await _tokenService.SaveUserTokenAsync(user.Id, "CustomProvider", "access_token", writtenAccessToken);
                await _tokenService.SaveUserTokenAsync(user.Id, "CustomProvider", "refresh_token", refreshToken);

            }
            else
            {
                loginResponseDTO.Errors.Add("Invalid login attempt.");
            }

            return loginResponseDTO;
        }


        //public async Task<Login_ResponseDTO> LoginAsync(Login_RequestDTO loginRequestDTO) // Uden refresh token og IP-adresse
        //{
        //    var result = await _signInManager.PasswordSignInAsync(
        //        loginRequestDTO.UserName,
        //        loginRequestDTO.Password,
        //        loginRequestDTO.RememberMe,
        //        lockoutOnFailure: false);

        //    var loginResponseDTO = new Login_ResponseDTO();

        //    if (result.Succeeded)
        //    {
        //        var user = await _userManager.FindByNameAsync(loginRequestDTO.UserName);

        //        // Generate the token with the user's claims
        //        var token = await GenerateToken(user);
        //        var tokenHandler = new JwtSecurityTokenHandler();

        //        loginResponseDTO.UserName = user.UserName;
        //        loginResponseDTO.Token = tokenHandler.WriteToken(token);
        //        loginResponseDTO.ExpiryDate = token.ValidTo;
        //    }
        //    else
        //    {
        //        loginResponseDTO.Errors.Add("Invalid login attempt.");
        //    }

        //    return loginResponseDTO;
        //}

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }


        //---------------------------------------: TOKEN SETUP :---------------------------------------

        /// <summary>
        /// Udsteder et nyt access token og refresh token baseret på et eksisterende refresh token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public async Task<Token_ResponseDTO> Get_NewAccessRefreshToken_FromRefreshTokenAsync(string userId, string refreshToken)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return null; // Brugeren blev ikke fundet
            }

            // Hent det gemte refresh token for brugeren
            var storedRefreshToken = await _userManager.GetAuthenticationTokenAsync(user, "CustomProvider", "refresh_token");

            if (storedRefreshToken != refreshToken || string.IsNullOrEmpty(storedRefreshToken))
            {
                return null; // Refresh token matcher ikke eller er ikke fundet
            }

            // Valider det gemte refresh token her (f.eks. tjek udløb, tilbagekaldelse osv.)

            // Generer et nyt access token og et nyt refresh token
            var newAccessToken = await _tokenService.GenerateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            var tokenHandler = new JwtSecurityTokenHandler();
            var writtenAccessToken = tokenHandler.WriteToken(newAccessToken);

            // Opdater det gemte refresh token med det nye refresh token
            await _tokenService.SaveUserTokenAsync(user.Id, "CustomProvider", "refresh_token", newRefreshToken);

            // Returner det nye access token og refresh token
            return new Token_ResponseDTO
            {
                AccessToken = writtenAccessToken,
                RefreshToken = newRefreshToken
            };
        }




        //---------------------------------------: EXTERNAL LOGIN :---------------------------------------

        public async Task<string> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                // Håndter fejl fra den eksterne udbyder
                throw new Exception($"Error from external provider: {remoteError}");
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                throw new Exception("Error loading external login information.");
            }

            // Forsøg at logge ind med den eksterne udbyder
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                // Brugeren er allerede registreret med denne eksterne udbyder
                var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                var accessToken = info.AuthenticationTokens.FirstOrDefault(t => t.Name == "access_token")?.Value;

                if (!string.IsNullOrEmpty(accessToken))
                {
                    // Gem access token i databasen
                    await _tokenService.SaveUserTokenAsync(user.Id, info.LoginProvider, "access_token", accessToken);
                }

                return await GenerateAndReturnToken(user, returnUrl);
            }
            else
            {
                // Brugeren er ikke registreret, så vi skal oprette en ny bruger
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                if (email == null)
                {
                    // Håndter scenarier, hvor e-mail ikke er tilgængelig
                    throw new Exception("Email not found.");
                }

                var user = new User { UserName = email, Email = email };
                var createUserResult = await _userManager.CreateAsync(user);
                if (!createUserResult.Succeeded)
                {
                    throw new Exception("Failed to create user.");
                }

                var addLoginResult = await _userManager.AddLoginAsync(user, info);
                if (!addLoginResult.Succeeded)
                {
                    throw new Exception("Failed to add external login.");
                }

                await _signInManager.SignInAsync(user, isPersistent: false);
                return await GenerateAndReturnToken(user, returnUrl);
            }
        }

        private async Task<string> GenerateAndReturnToken(User user, string returnUrl)
        {
            var accessToken = await _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();
            await _tokenService.UpdateRefreshTokenForUser(user, refreshToken, "Brugerens IP"); // Erstat "Brugerens IP" med den faktiske IP

            var tokenHandler = new JwtSecurityTokenHandler();
            var accessToken_ToString = tokenHandler.WriteToken(accessToken);

            // Her kan du vælge at returnere token direkte, omdirigere brugeren med token som en parameter, eller en anden metode
            // For eksempel, for en SPA, kan du returnere en URL med tokenet som en query parameter
            // For denne demonstration returnerer vi bare tokenet som en string
            return accessToken_ToString;
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