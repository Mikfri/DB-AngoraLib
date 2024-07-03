using DB_AngoraLib.DTOs;
using DB_AngoraLib.EF_DbContext;
using DB_AngoraLib.Models;
using DB_AngoraLib.Repository;
using DB_AngoraLib.Services.AccountService;
using DB_AngoraLib.Services.RabbitService;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Services.ApplicationServices
{
    public class ApplicationServices : IApplicationService
    {
        private readonly IGRepository<BreederApplication> _dbRepository;
        private readonly IRabbitService _rabbitServices;
        private readonly IAccountService _accountServices;
        private readonly UserManager<User> _userManager;

        public ApplicationServices(IGRepository<BreederApplication> breederAppRepository, IRabbitService rabbitService, IAccountService accountService, UserManager<User> userManager)
        {
            _dbRepository = breederAppRepository;
            _rabbitServices = rabbitService;
            _accountServices = accountService;
            _userManager = userManager;
        }


        //---------------------------------: CREATE/POST :---------------------------------
        //----------------: Create application
        public async Task ApplyForBreederRoleAsync(string userId, Application_BreederDTO applicationDTO)
        {
            // Brug GetUserByIdAsync fra IAccountService til at hente den aktuelt loggede ind brugers oplysninger
            var user = await _accountServices.Get_UserByIdAsync(userId);
            if (user == null) throw new Exception("User not found");

            if (user.BreederRegNo is not null)
            {
                throw new Exception("Du er allerede registreret som avler");
            }

            var existingApplication = await _dbRepository.GetObject_ByFilterAsync(ba => ba.UserId == userId && ba.Status == RequestStatus.Pending);
            if (existingApplication != null)
            {
                throw new Exception("Du har allerede en afventende ansøgning om optagelse som avler! Vent venligst");
            }

            var application = new BreederApplication
            {
                UserId = userId,
                RequestedBreederRegNo = applicationDTO.RequestedBreederRegNo,
                DocumentationPath = applicationDTO.DocumentationPath,
                Status = RequestStatus.Pending
            };

            await _dbRepository.AddObjectAsync(application);
        }



        //----------------: Approve application (Create 'Breeder')
        public async Task ApproveApplicationAsync(int applicationId)
        {
            // Find ansøgningen ved hjælp af GRepository
            var application = await _dbRepository.GetObject_ByIntKEYAsync(applicationId);
            if (application == null) throw new Exception("Application not found");

            // Opdater ansøgningens status
            application.Status = RequestStatus.Approved;

            // Find brugeren og opdater deres rolle og BreederRegNo
            var user = await _accountServices.Get_UserByIdAsync(application.UserId);
            if (user == null) throw new Exception("User not found");

            var addToRoleResult = await _userManager.AddToRoleAsync(user, "Breeder");
            if (!addToRoleResult.Succeeded)
            {
                throw new Exception("Failed to add user to Breeder role");
            }

            user.BreederRegNo = application.RequestedBreederRegNo;

            // Gem ændringerne i ansøgningen
            await _dbRepository.UpdateObjectAsync(application);

            // Link relevante kaniner til den nye avler
            await _rabbitServices.LinkRabbits_ToNewBreederAsync(user.BreederRegNo, user.Id);
        }

        //----------------: Reject application
        public async Task RejectApplicationAsync(int applicationId, string reason)
        {
            // Find ansøgningen ved hjælp af GRepository
            var application = await _dbRepository.GetObject_ByIntKEYAsync(applicationId);
            if (application == null) throw new Exception("Application not found");

            // Opdater ansøgningens status og tilføj afvisningsårsagen
            application.Status = RequestStatus.Rejected;
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
            return pendingApplications.Where(a => a.Status == RequestStatus.Pending);
        }

    }
}
