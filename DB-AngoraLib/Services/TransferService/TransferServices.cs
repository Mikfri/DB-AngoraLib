using DB_AngoraLib.DTOs;
using DB_AngoraLib.Models;
using DB_AngoraLib.Repository;
using DB_AngoraLib.Services.AccountService;
using DB_AngoraLib.Services.RabbitService;
using DB_AngoraLib.Services.ValidationService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Services.TransferService
{
    public class TransferServices
    {

        private readonly IGRepository<RabbitTransfer> _dbRepository;
        private readonly IRabbitService _rabbitServices;
        private readonly IAccountService _accountService;
        private readonly NotificationService _notificationService;

        public TransferServices(IGRepository<RabbitTransfer> dbRepository, IRabbitService rabbitService, IAccountService userService, NotificationService notificationService)
        {
            _dbRepository = dbRepository;
            _rabbitServices = rabbitService;
            _accountService = userService;
            _notificationService = notificationService;
        }

        //----------------------------------------------: CREATE :----------------------------------------------
        public async Task<RabbitTransfer_ContractDTO> CreateTransferRequest(string userId, RabbitTransfer_CreateDTO createTransferDTO)
        {
            // Valider input (Dette trin kan udvides med mere detaljerede valideringer)
            if (createTransferDTO == null)
                throw new ArgumentNullException(nameof(createTransferDTO));

            // Tjek for eksisterende kanin
            var rabbit = await _rabbitServices.Get_Rabbit_ByEarCombId(createTransferDTO.EarCombId) 
                ?? throw new ArgumentException("Kaninen blev ikke fundet");

            // Tjek for eksisterende nuværende ejer
            var currentOwner = await _accountService.Get_UserByIdAsync(userId)
                ?? throw new ArgumentException("Nuværende ejer blev ikke fundet.");

            if (rabbit.OwnerId != userId)
                throw new InvalidOperationException("Du er ikke den nuværende ejer af denne kanin.");

            var proposedOwner = await _accountService.Get_UserByBreederRegNoAsync(createTransferDTO.ProposedOwner_BreederRegNo)
                ?? throw new ArgumentException("Foreslået ejer blev ikke fundet.");

            // Tjek for eksisterende aktive RabbitTransfer anmodninger for den samme kanin
            var existingTransfer = await _dbRepository.GetDbSet()
                .FirstOrDefaultAsync(rt => rt.RabbitId == rabbit.EarCombId && rt.Status == RequestStatus.Pending);

            if (existingTransfer is not null)
            {
                throw new InvalidOperationException($"Der findes allerede en aktiv overførselsanmodning for denne kanin. (RabbitTransfer.{existingTransfer.Id})");
            }

            // Opret overførselsanmodning
            var newTransferRequest = new RabbitTransfer
            {
                Status = RequestStatus.Pending,

                CurrentOwnerId = userId,
                RabbitId = rabbit.EarCombId,
                Price = createTransferDTO.Price,
                SaleConditions = createTransferDTO.SaleConditions,
                ProposedOwnerId = proposedOwner.Id,
                //ResponseDate = null,
            };

            await _dbRepository.AddObjectAsync(newTransferRequest);
            
            await _notificationService.CreateNotificationAsync(
                proposedOwner.Id, $"Ny anmodning om at overtagning af ejerskab for kaninen {rabbit.EarCombId}: {rabbit.NickName}");


            // Returner en bekræftelse
            return new RabbitTransfer_ContractDTO
            {
                Id = newTransferRequest.Id,
                Status = newTransferRequest.Status,

                CurrentOwner_BreederRegNo = currentOwner.BreederRegNo,
                EarCombId = newTransferRequest.RabbitId,
                Price = newTransferRequest.Price,
                SaleConditions = newTransferRequest.SaleConditions,
                ProposedOwner_BreederRegNo = proposedOwner.BreederRegNo,
            };
        }


        //----------------------------------------------: GET/READ :----------------------------------------------

        public async Task<RabbitTransfer_ContractDTO> Get_RabbitTransfer_Contract(string userId, int transferId)
        {
            var transfer = await _dbRepository.GetDbSet()
                .Include(rt => rt.RabbitInTrans)
                .Include(rt => rt.UserCurrentOwner.BreederRegNo)
                .Include(rt => rt.UserProposedOwner.BreederRegNo)
                .FirstOrDefaultAsync(rt => rt.Id == transferId);

            var user = await _accountService.Get_UserByIdAsync(userId)
                ?? throw new ArgumentException("Bruger ikke fundet.");

            if (userId != transfer.CurrentOwnerId && userId != transfer.ProposedOwnerId)
            {
                throw new InvalidOperationException("Du har ikke tilladelse til at se denne anmodning.");
            }

            if (transfer == null)
            {
                throw new ArgumentException("Overførselsanmodning ikke fundet.");
            }

            if (transfer.Status == RequestStatus.Pending)
            {
                return new RabbitTransfer_ContractDTO
                {
                    Id = transfer.Id,
                    Status = transfer.Status,
                    CurrentOwner_BreederRegNo = transfer.UserCurrentOwner.BreederRegNo,

                    Issuer_FullName = $"{transfer.UserCurrentOwner.FirstName} {transfer.UserCurrentOwner.LastName}",
                    Issuer_Email = transfer.UserCurrentOwner.Email,
                    Issuer_RoadNameAndNo = transfer.UserCurrentOwner.RoadNameAndNo,
                    Issuer_ZipCode = transfer.UserCurrentOwner.ZipCode,
                    Issuer_City = transfer.UserCurrentOwner.City,

                    EarCombId = transfer.RabbitInTrans.EarCombId,
                    Price = transfer.Price,
                    SaleConditions = transfer.SaleConditions,

                    ProposedOwner_BreederRegNo = transfer.UserProposedOwner.BreederRegNo,
                    DateAccepted = transfer.DateAccepted,
                };
            }
            else
            {
                return new RabbitTransfer_ContractDTO
                {
                    Id = transfer.Id,
                    Status = transfer.Status,
                    CurrentOwner_BreederRegNo = transfer.UserCurrentOwner.BreederRegNo,

                    Issuer_FullName = $"{transfer.UserCurrentOwner.FirstName} {transfer.UserCurrentOwner.LastName}",
                    Issuer_Email = transfer.UserCurrentOwner.Email,
                    Issuer_RoadNameAndNo = transfer.UserCurrentOwner.RoadNameAndNo,
                    Issuer_ZipCode = transfer.UserCurrentOwner.ZipCode,
                    Issuer_City = transfer.UserCurrentOwner.City,

                    EarCombId = transfer.RabbitInTrans.EarCombId,
                    Price = transfer.Price,
                    SaleConditions = transfer.SaleConditions,
                    ProposedOwner_BreederRegNo = transfer.UserProposedOwner.BreederRegNo,
                    DateAccepted = transfer.DateAccepted,

                    Recipent_FullName = $"{transfer.UserProposedOwner.FirstName} {transfer.UserProposedOwner.LastName}",
                    Recipent_Email = transfer.UserProposedOwner.Email,
                    Recipent_RoadNameAndNo = transfer.UserProposedOwner.RoadNameAndNo,
                    Recipent_ZipCode = transfer.UserProposedOwner.ZipCode,
                    Recipent_City = transfer.UserProposedOwner.City,
                };
            }
        }

        public async Task<RabbitTransfer_ContractDTO> Respond_TransferRequest(string proposedUserId, int transferId, bool accept)
        {
            var transferReq = await _dbRepository.GetDbSet()
                .Include(rt => rt.RabbitInTrans)
                .Include(rt => rt.UserCurrentOwner.BreederRegNo)
                .Include(rt => rt.UserProposedOwner.BreederRegNo)
                .FirstOrDefaultAsync(rt => rt.Id == transferId);

            var proposedOwner = await _accountService.Get_UserByIdAsync(proposedUserId);

            if (transferReq == null)
            {
                throw new ArgumentException("Overførselsanmodning ikke fundet.");
            }

            if (transferReq.ProposedOwnerId != proposedUserId)
            {
                throw new InvalidOperationException("Du har ikke tilladelse til at svare på denne anmodning.");
            }

            if (transferReq.Status != RequestStatus.Pending)
            {
                throw new InvalidOperationException("Denne anmodning er ikke længere aktiv.");
            }

            transferReq.Status = accept ? RequestStatus.Approved : RequestStatus.Rejected;

            if (accept)
            {
                transferReq.Status = RequestStatus.Approved;
                transferReq.RabbitInTrans.OwnerId = transferReq.ProposedOwnerId;
                transferReq.DateAccepted = DateOnly.FromDateTime(DateTime.Now); // Sætter den aktuelle dato

                await _rabbitServices.UpdateRabbitOwnershipAsync(transferReq.RabbitInTrans);

                // Notify current owner
                await _notificationService.CreateNotificationAsync(transferReq.CurrentOwnerId,
                    $"Din anmodning om at overføre ejerskab af kaninen {transferReq.RabbitInTrans.NickName} er blevet accepteret");
                
                return Get_RabbitTransfer_Contract(proposedUserId, transferId).Result;
            }
            else
            {
                transferReq.Status = RequestStatus.Rejected;

                // Notify current owner
                await _notificationService.CreateNotificationAsync(transferReq.CurrentOwnerId,
                    $"Din anmodning om at overføre ejerskab af kaninen {transferReq.RabbitInTrans.NickName} er blevet afvist");

                // Delete transfer
                await _dbRepository.DeleteObjectAsync(transferReq);

                return null;
            }
        }


        //---------------------------------: HELPER METHODS :---------------------------------

    }
}
