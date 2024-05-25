using DB_AngoraLib.DTOs;
using DB_AngoraLib.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
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

        
        public async Task<Login_ResponseDTO> LoginAsync(Login_RequestDTO loginRequest)
        {
            var result = await _signInManager.PasswordSignInAsync(loginRequest.UserName, loginRequest.Password, loginRequest.RememberMe, lockoutOnFailure: false);

            var signinResultResponse = new Login_ResponseDTO();

            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(loginRequest.UserName);
                await SynchronizeUserClaims(user);

                var claims = await _userManager.GetClaimsAsync(user);

                // Generate the token with the user's claims
                var token = await GenerateToken(user, claims);
                var tokenHandler = new JwtSecurityTokenHandler();

                signinResultResponse.UserName = user.UserName;
                signinResultResponse.Token = tokenHandler.WriteToken(token);
                signinResultResponse.ExpiryDate = token.ValidTo;
            }
            else
            {
                signinResultResponse.Errors.Add("Invalid login attempt.");
            }

            return signinResultResponse;
        }


        private async Task<JwtSecurityToken> GenerateToken(User user, IList<Claim> claims)
        {
            var keyString = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Add the four claims
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));

            // Get the actual role of the user
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            claims.Add(new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"));
            claims.Add(new Claim(ClaimTypes.Email, user.Email));

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return token;
        }



        public async Task SynchronizeUserClaims(User user) // TODO: Benyt evt denne til LoginAsync
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var allUserRoleClaims = new List<Claim>();

            // Laver en liste af af RoleClaims baseret på brugerens roller
            foreach (var roleName in userRoles)
            {
                var aConfirmedUserRole = await _roleManager.FindByNameAsync(roleName);
                var confirmedRoleClaims = await _roleManager.GetClaimsAsync(aConfirmedUserRole);
                allUserRoleClaims.AddRange(confirmedRoleClaims);
            }

            var userClaims = await _userManager.GetClaimsAsync(user);

            // For hver UserClaim, brugeren har
            foreach (var claim in userClaims)
            {
                // Hvis der findes UserClaims som ikke stemmer overens med brugerens liste af RoleClaims
                if (!allUserRoleClaims.Any(rc => rc.Type == claim.Type && rc.Value == claim.Value)) /* && !userClaims.Any(uc => uc.Type == "SpecialPermission" && uc.Value == claim.Value))*/
                {
                    // Fjerner specifikke UserClaim fra brugeren
                    await _userManager.RemoveClaimAsync(user, claim);
                }
            }
            // For hver Claim i brugerens liste af RoleClaims
            foreach (var claim in allUserRoleClaims)
            {
                // Hvis brugeren mangler en UserClaim, som findes i brugeren liste af RoleClaims
                if (!userClaims.Any(uc => uc.Type == claim.Type && uc.Value == claim.Value))
                {
                    // Tilføjer UserClaim til brugeren
                    await _userManager.AddClaimAsync(user, claim);
                }
            }
        }


        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

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