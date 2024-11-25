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
                // AccessToken er nu direkte en string
                var accessToken = await _tokenService.GenerateAccessToken(user);
                var refreshToken = _tokenService.GenerateRefreshToken();

                await _tokenService.UpdateRefreshTokenForUser(user, refreshToken, userIP);

                // Ikke længere behov for JwtSecurityTokenHandler
                loginResponseDTO.UserName = user.UserName;
                loginResponseDTO.AccessToken = accessToken; // Direkte brug af string
                                                            // Vi skal parse token for at få ValidTo - eller måske ændre DTO'en til ikke at include ExpiryDate
                loginResponseDTO.RefreshToken = refreshToken;

                await _tokenService.SaveUserTokenAsync(user.Id, "CustomProvider", "access_token", accessToken);
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

            // AccessToken er nu direkte en string
            var newAccessToken = await _tokenService.GenerateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            // Opdater det gemte refresh token med det nye refresh token
            await _tokenService.SaveUserTokenAsync(user.Id, "CustomProvider", "refresh_token", newRefreshToken);
            // Returner det nye access token og refresh token
            return new Token_ResponseDTO
            {
                AccessToken = newAccessToken, // Direkte brug af string
                RefreshToken = newRefreshToken
            };
        }


        //---------------------------------------: EXTERNAL LOGIN :---------------------------------------
        public async Task<Login_ResponseDTO> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
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
                // Brugeren er allerede registreret, så vi udsteder et nyt Bearer Token
                var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                // Udsted dit eget Bearer Token for brugeren
                var accessToken = await _tokenService.GenerateAccessToken(user);
                var refreshToken = _tokenService.GenerateRefreshToken();

                await _tokenService.UpdateRefreshTokenForUser(user, refreshToken, "Brugerens IP");

                // Returner dit eget token til brugeren
                return new Login_ResponseDTO
                {
                    UserName = user.UserName,
                    AccessToken = accessToken, // Direkte brug af string
                    RefreshToken = refreshToken,
                };
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

                // Udsted dit eget Bearer Token for brugeren
                var accessToken = await _tokenService.GenerateAccessToken(user);
                var refreshToken = _tokenService.GenerateRefreshToken();

                await _tokenService.UpdateRefreshTokenForUser(user, refreshToken, "Brugerens IP");

                return new Login_ResponseDTO
                {
                    UserName = user.UserName,
                    AccessToken = accessToken, // Direkte brug af string
                    RefreshToken = refreshToken,
                };
            }
        }
    }
}