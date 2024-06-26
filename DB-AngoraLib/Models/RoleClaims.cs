using DB_AngoraLib.MockData;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;

public static class RoleClaims
{
    private static List<IdentityRoleClaim<string>> _roleClaimsList = new List<IdentityRoleClaim<string>>();

    static RoleClaims()
    {
        var rolePermissions = new List<(string ClaimType, string ClaimValue, List<string> Roles)>
        {
            // TODO: Kan vi bruge enums for Role, ClaimType og ClaimValue?
            ("User:Read", "Any", new List<string> { "Admin" }),
            ("User:Create", "Any", new List<string> { "Admin" }),
            ("User:Update", "Any", new List<string> { "Admin" }),
            ("User:Delete", "Any", new List<string> { "Admin" }),

            ("Rabbit:Create", "Any", new List<string> { "Admin", "Moderator" }),
            ("Rabbit:Read", "Any", new List<string> { "Admin", "Moderator" }),
            ("Rabbit:Update", "Any", new List<string> { "Admin", "Moderator" }),
            ("Rabbit:Delete", "Any", new List<string> { "Admin", "Moderator" }),

            ("Rabbit:Create", "Own", new List<string> { "Breeder" }),
            ("Rabbit:Read", "Own", new List<string> { "Breeder" }),
            ("Rabbit:Update", "Own", new List<string> { "Breeder" }),
            ("Rabbit:Delete", "Own", new List<string> { "Breeder" })
        };

        foreach (var permission in rolePermissions)
        {
            foreach (var roleName in permission.Roles)
            {
                AddRoleClaim(roleName, permission.ClaimType, permission.ClaimValue);
            }
        }
    }

    private static void AddRoleClaim(string roleName, string claimType, string claimValue)
    {
        var roleId = MockRoles.GetMockRoles().First(r => r.Name == roleName).Id;
        _roleClaimsList.Add(new IdentityRoleClaim<string> { RoleId = roleId, ClaimType = claimType, ClaimValue = claimValue });
    }

    public static List<IdentityRoleClaim<string>> Get_AspNetRoleClaims()
    {
        return _roleClaimsList;
    }
}
