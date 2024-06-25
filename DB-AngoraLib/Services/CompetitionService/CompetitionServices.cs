using DB_AngoraLib.Models;
using DB_AngoraLib.Repository;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Services.CompetitionService
{
    public class CompetitionServices
    {
        private readonly IGRepository<User> _dbRepository;
        private readonly UserManager<User> _userManager;

        public CompetitionServices(IGRepository<User> dbRepository, UserManager<User> userManager)
        {
            _dbRepository = dbRepository;
            _userManager = userManager;
        }

        public async Task AssignJudgeToCompetition(string userId, string competitionId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var claim = new Claim("CompetitionAccess", competitionId);
            await _userManager.AddClaimAsync(user, claim);
        }

        public async Task RemoveJudgeFromCompetition(string userId, string competitionId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var claim = new Claim("CompetitionAccess", competitionId);
            await _userManager.RemoveClaimAsync(user, claim);
        }

        //public async Task<bool> CanJudgeAccessCompetition(string userId, string competitionId)
        //{
        //    var user = await _userManager.FindByIdAsync(userId);
        //    var claims = await _userManager.GetClaimsAsync(user);
        //    var hasAccess = claims.Any(c => c.Type == "CompetitionAccess" && c.Value == competitionId);

        //    // Antag at du har en metode til at tjekke, om stævnet er aktivt
        //    var isCompetitionActive = IsCompetitionActive(competitionId);

        //    return hasAccess && isCompetitionActive;
        //}
    }
}
