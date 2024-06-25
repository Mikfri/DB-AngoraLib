using DB_AngoraLib.EF_DbContext;
using DB_AngoraLib.Models;
using DB_AngoraLib.Repository;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Services.ApplicationServices
{
    public class ApplicationServices
    {
        private readonly IGRepository<BreederApplication> _dbRepository;
        private readonly UserManager<User> _userManager;

        public ApplicationServices(IGRepository<BreederApplication> breederAppRepository, UserManager<User> userManager)
        {
            _dbRepository = breederAppRepository;
            _userManager = userManager;
        }


        //---------------------------------: CREATE/POST :---------------------------------
        //----------------: Create application
        public async Task ApplyForBreederRoleAsync(string userId, string requestedBreederRegNo, string documentationPath)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new Exception("User not found");

            if (user.BreederRegNo is not null)
            {
                throw new Exception("Du er allerede registreret som avler");
            }

            var existingApplication = await _dbRepository.GetObject_ByFilterAsync(ba => ba.UserId == userId && ba.Status == BreederRequestStatus.Pending);
            if (existingApplication != null)
            {
                throw new Exception("Du har allerede en afventende ansøgning om optagelse som avler! Vent venligst");
            }

            var application = new BreederApplication
            {
                UserId = userId,
                RequestedBreederRegNo = requestedBreederRegNo,
                DocumentationPath = documentationPath,
                Status = BreederRequestStatus.Pending
            };

            await _dbRepository.AddObjectAsync(application);
        }

        //----------------: Create 'Breeder'
        public async Task ApproveApplicationAsync(int applicationId)
        {
            // Find ansøgningen ved hjælp af GRepository
            var application = await _dbRepository.GetObject_ByKEYAsync(applicationId.ToString());
            if (application == null) throw new Exception("Application not found");

            // Opdater ansøgningens status
            application.Status = BreederRequestStatus.Approved;

            // Find brugeren og opdater deres rolle og BreederRegNo
            var user = await _userManager.FindByIdAsync(application.UserId);
            if (user == null) throw new Exception("User not found");

            var addToRoleResult = await _userManager.AddToRoleAsync(user, "Breeder");
            if (!addToRoleResult.Succeeded)
            {
                throw new Exception("Failed to add user to Breeder role");
            }

            user.BreederRegNo = application.RequestedBreederRegNo;

            // Gem ændringerne i ansøgningen
            await _dbRepository.UpdateObjectAsync(application);
        }

        //----------------: Reject application
        public async Task RejectApplicationAsync(int applicationId, string reason)
        {
            // Find ansøgningen ved hjælp af GRepository
            var application = await _dbRepository.GetObject_ByKEYAsync(applicationId.ToString());
            if (application == null) throw new Exception("Application not found");

            // Opdater ansøgningens status og tilføj afvisningsårsagen
            application.Status = BreederRequestStatus.Rejected;
            application.RejectionReason = reason;

            // Gem ændringerne i ansøgningen
            await _dbRepository.UpdateObjectAsync(application);
        }

        //---------------------------------: READ/GET :---------------------------------
        //----------------: Get pending applications
        public async Task<IEnumerable<BreederApplication>> GetPendingApplicationsAsync()
        {
            // Brug _dbRepository til at hente alle ansøgninger med status 'Pending'
            var pendingApplications = await _dbRepository.GetAllObjectsAsync();
            return pendingApplications.Where(a => a.Status == BreederRequestStatus.Pending);
        }

    }
}
