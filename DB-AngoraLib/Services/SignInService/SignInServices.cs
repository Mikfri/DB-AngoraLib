using DB_AngoraLib.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DB_AngoraLib.Services.SigninService
{
    public class SigninServices : ISigninService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;

        public SigninServices(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public async Task<SignInResult> LoginAsync(string userName, string password, bool rememberMe)
        {
            var result = await _signInManager.PasswordSignInAsync(userName, password, rememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(userName);
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
                    if (!allUserRoleClaims.Any(rc => rc.Type == claim.Type && rc.Value == claim.Value))
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
            return result;
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
                if (!allUserRoleClaims.Any(rc => rc.Type == claim.Type && rc.Value == claim.Value) && !userClaims.Any(uc => uc.Type == "ExtraClaims" && uc.Value == claim.Value))
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