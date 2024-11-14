﻿using DB_AngoraLib.DTOs;
using DB_AngoraLib.EF_DbContext;
using DB_AngoraLib.Models;
using DB_AngoraLib.Repository;
using DB_AngoraLib.Services.AccountService;
using DB_AngoraLib.Services.BreederBrandService;
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
        private readonly IGRepository<ApplicationBreeder> _dbRepository;
        private readonly IRabbitService _rabbitServices;
        private readonly IAccountService _accountServices;
        private readonly IBreederBrandService _breederBrandServices;
        private readonly UserManager<User> _userManager;

        public ApplicationServices(IGRepository<ApplicationBreeder> breederAppRepository, IRabbitService rabbitService, IAccountService accountService, IBreederBrandService breederBrandServices, UserManager<User> userManager)
        {
            _dbRepository = breederAppRepository;
            _rabbitServices = rabbitService;
            _accountServices = accountService;
            _breederBrandServices = breederBrandServices;
            _userManager = userManager;
        }


        //---------------------------------: CREATE/POST :---------------------------------
        //----------------: Create application

        //TODO: Skal returnere en DTO for at opnå 201 Created
        /// <summary>
        /// Lader en User med 'Guest' rolle ansøge om at få en 'Breeder' rolle. 
        /// Ansøgningnen valideres af en Moderator eller af Admin
        /// </summary>
        /// <param name="userId">Brugeren som er logget på</param>
        /// <param name="applicationDTO"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task Apply_ApplicationBreeder(string userId, ApplicationBreeder_CreateDTO applicationDTO)
        {
            // Brug GetUserByIdAsync fra IAccountService til at hente den aktuelt loggede ind brugers oplysninger
            var user = await _accountServices.Get_UserById(userId);
            if (user == null) throw new Exception("User not found");

            // Tjek om brugeren allerede er en Breeder
            var isBreeder = await _userManager.IsInRoleAsync(user, "Breeder");
            if (isBreeder)
            {
                throw new Exception("Du er allerede registreret som avler");
            }

            var existingApplication = await _dbRepository.GetObject_ByFilterAsync(ba => ba.UserApplicantId == userId && ba.Status == ApplicationStatus.Pending);
            if (existingApplication is not null) 
            {
                throw new Exception("Du har allerede en afventende ansøgning om optagelse som avler! Vent venligst");
            }

            var application = new ApplicationBreeder
            {
                UserApplicantId = userId,
                RequestedBreederRegNo = applicationDTO.RequestedBreederRegNo,
                DocumentationPath = applicationDTO.DocumentationPath,
                Status = ApplicationStatus.Pending
            };

            await _dbRepository.AddObjectAsync(application);
        }



        // TODO: Se om den pågældende bruger skal modtage en notifikation/email om afvisning/success
        public async Task Respond_ApplicationBreeder(int applicationId, ApplicationBreeder_ResponseDTO responseDTO)
        {
            var application = await _dbRepository.GetObject_ByIntKEYAsync(applicationId);
            if (application is null) throw new Exception("Ansøgning ikke fundet");

            if (responseDTO.IsApproved)
            {
                application.Status = ApplicationStatus.Approved;

                var user = await _accountServices.Get_UserById(application.UserApplicantId);
                if (user is null) throw new Exception("Bruger ikke fundet");

                // Opdater discriminator kolonnen til "Breeder"
                user.GetType().GetProperty("UserType").SetValue(user, "Breeder");

                // Tilføj Breeder-specifikke data
                var breeder = user as Breeder;
                if (breeder == null)
                {
                    breeder = new Breeder
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        RoadNameAndNo = user.RoadNameAndNo,
                        ZipCode = user.ZipCode,
                        City = user.City,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        PasswordHash = user.PasswordHash,
                        BreederRegNo = application.RequestedBreederRegNo
                    };
                }
                else // TODO: Se om else blokken kan fjernes
                {
                    breeder.BreederRegNo = application.RequestedBreederRegNo;
                }

                // Gem ændringerne
                var updateResult = await _userManager.UpdateAsync(breeder);
                if (!updateResult.Succeeded)
                {
                    throw new Exception("Kunne ikke opdatere brugeren til typen 'Breeder'");
                }

                // Tildel Breeder rollen
                var addToRoleResult = await _userManager.AddToRoleAsync(breeder, "BreederBasic");
                if (!addToRoleResult.Succeeded)
                {
                    throw new Exception("Brugerollen 'Breeder' kunne ikke tildeles");
                }

                await _rabbitServices.LinkRabbits_ToNewBreederAsync(breeder.Id, breeder.BreederRegNo);
                await _breederBrandServices.Create_BreederBrand(breeder.Id);
            }
            else
            {
                application.Status = ApplicationStatus.Rejected;
                application.RejectionReason = responseDTO.RejectionReason ?? "Venligst kontakt DB-Angoras service på yyy@yyy.dk, for hjælp";
            }

            await _dbRepository.UpdateObjectAsync(application);
        }



        //---------------------------------: READ/GET :---------------------------------
        //----------------: Get pending applications

        // TODO: Skal returnere en DTO i stedet for modellen
        public async Task<IEnumerable<ApplicationBreeder>> GetAll_ApplicationBreeder_Pending()
        {
            var pendingApplications = await _dbRepository.GetAllObjectsAsync();
            return pendingApplications.Where(a => a.Status == ApplicationStatus.Pending);
        }

    }
}
