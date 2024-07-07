using DB_AngoraLib.DTOs;
using DB_AngoraLib.Models;
using DB_AngoraLib.Repository;
using DB_AngoraLib.Services.AccountService;
using DB_AngoraLib.Services.RabbitService;
using DB_AngoraLib.Services.ValidationService;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Services.TransferService
{
    public class TransferServices : ITransferService
    {

        private readonly IGRepository<TransferRequst> _dbRepository;
        private readonly IRabbitService _rabbitServices;
        private readonly IAccountService _accountService;
        private readonly NotificationService _notificationService;

        public TransferServices(IGRepository<TransferRequst> dbRepository, IRabbitService rabbitService, IAccountService userService, NotificationService notificationService)
        {
            _dbRepository = dbRepository;
            _rabbitServices = rabbitService;
            _accountService = userService;
            _notificationService = notificationService;
        }

        //----------------------------------------------: CREATE :----------------------------------------------
        public async Task<TransferRequest_ContractDTO> CreateTransferRequest(string issuerId, TransferRequest_CreateDTO createTransferDTO)
        //public async Task<TransferRequest_PreviewDTO> CreateTransferRequest(string issuerId, TransferRequest_CreateDTO createTransferDTO)
        {
            // Valider input (Dette trin kan udvides med mere detaljerede valideringer)
            if (createTransferDTO == null)
                throw new ArgumentNullException(nameof(createTransferDTO));

            // Tjek for eksisterende kanin
            var rabbit = await _rabbitServices.Get_Rabbit_ByEarCombId(createTransferDTO.Rabbit_EarCombId) 
                ?? throw new ArgumentException("Kaninen blev ikke fundet");

            // Tjek for eksisterende nuværende ejer
            var userIssuer = await _accountService.Get_UserByIdAsync(issuerId)
                ?? throw new ArgumentException("Nuværende ejer blev ikke fundet");

            if (rabbit.OwnerId != issuerId)
                throw new InvalidOperationException("Du er ikke den nuværende ejer af denne kanin");

            var userRecipent = await _accountService.Get_UserByBreederRegNoAsync(createTransferDTO.Recipent_BreederRegNo)
                ?? throw new ArgumentException("Foreslået ejer blev ikke fundet");

            // Tjek for eksisterende aktive RabbitTransfer anmodninger for den samme kanin
            var existingTransfer = await _dbRepository.GetDbSet()
                .FirstOrDefaultAsync(rt => rt.RabbitId == rabbit.EarCombId && rt.Status == RequestStatus.Pending);

            if (existingTransfer is not null)
            {
                throw new InvalidOperationException($"Der findes allerede en aktiv overførselsanmodning for denne kanin. (TransferRequest.{existingTransfer.Id})");
            }

            // Opret overførselsanmodning
            var newTransferRequest = new TransferRequst
            {
                IssuerId = issuerId,

                RabbitId = createTransferDTO.Rabbit_EarCombId,
                RecipentId = userRecipent.Id,
                Price = createTransferDTO.Price,

                SaleConditions = createTransferDTO.SaleConditions,
                Status = RequestStatus.Pending,
                DateAccepted = null,
            };

            await _dbRepository.AddObjectAsync(newTransferRequest);
            
            await _notificationService.CreateNotificationAsync(
                userRecipent.Id, $"Ny anmodning om at overtagning af ejerskab for kaninen {rabbit.EarCombId}: {rabbit.NickName}");

            //return new TransferRequest_PreviewDTO
            //{
            //    Id = newTransferRequest.Id,
            //    Status = newTransferRequest.Status,
            //    DateAccepted = newTransferRequest.DateAccepted,

            //    Rabbit_EarCombId = newTransferRequest.RabbitId,
            //    Issuer_BreederRegNo = userIssuer.BreederRegNo,
            //    Recipent_BreederRegNo = userRecipent.BreederRegNo,

            //    Price = newTransferRequest.Price,
            //    SaleConditions = newTransferRequest.SaleConditions,
            //};

            return new TransferRequest_ContractDTO
            {
                Id = newTransferRequest.Id,
                Status = newTransferRequest.Status,
                Issuer_BreederRegNo = newTransferRequest.UserIssuer.BreederRegNo,

                Issuer_FullName = $"{newTransferRequest.UserIssuer.FirstName} {newTransferRequest.UserIssuer.LastName}",
                Issuer_Email = newTransferRequest.UserIssuer.Email,
                Issuer_RoadNameAndNo = newTransferRequest.UserIssuer.RoadNameAndNo,
                Issuer_ZipCode = newTransferRequest.UserIssuer.ZipCode,
                Issuer_City = newTransferRequest.UserIssuer.City,

                Recipent_BreederRegNo = newTransferRequest.UserRecipent.BreederRegNo,
                DateAccepted = newTransferRequest.DateAccepted,

                Rabbit_EarCombId = newTransferRequest.Rabbit.EarCombId,
                Rabbit_NickName = newTransferRequest.Rabbit.NickName,
                Price = newTransferRequest.Price,
                SaleConditions = newTransferRequest.SaleConditions,
            };
        }


        public async Task<TransferRequest_ContractDTO> Response_TransferRequest(string recipentId, int transferId, TransferRequest_ResponseDTO responseDTO)
        {
            var transferReq = await Get_TransferRequest(transferId);

            if (transferReq.RecipentId != recipentId)
            {
                throw new InvalidOperationException("Du har ikke tilladelse til at svare på denne anmodning.");
            }

            if (transferReq.Status != RequestStatus.Pending)
            {
                throw new InvalidOperationException("Denne anmodning er ikke længere aktiv.");
            }

            transferReq.Status = responseDTO.Accept ? RequestStatus.Approved : RequestStatus.Rejected;

            if (responseDTO.Accept)
            {
                transferReq.Status = RequestStatus.Approved;
                transferReq.Rabbit.OwnerId = transferReq.RecipentId;
                transferReq.DateAccepted = DateOnly.FromDateTime(DateTime.Now); // Sætter den aktuelle dato

                await _rabbitServices.UpdateRabbitOwnershipAsync(transferReq.Rabbit);

                // Notify current owner
                await _notificationService.CreateNotificationAsync(transferReq.IssuerId,
                    $"Din anmodning om at overføre ejerskab af kaninen {transferReq.Rabbit.NickName} er blevet accepteret");

                return await Get_RabbitTransfer_Contract(recipentId, transferId);
            }
            else
            {
                // Brug den opdaterede DeleteTransferRequest metode
                await DeleteTransferRequest(recipentId, transferId);

                // Notify current owner
                await _notificationService.CreateNotificationAsync(transferReq.IssuerId,
                    $"Din anmodning om at overføre ejerskab af kaninen {transferReq.Rabbit.NickName} er blevet afvist");

                return null;
            }
        }

        //----------------------------------------------: GET/READ :----------------------------------------------

        private async Task<TransferRequst> Get_TransferRequest(int transferId)
        {
            var transferRequest = await _dbRepository.GetDbSet()
                .Include(rt => rt.Rabbit)
                .Include(rt => rt.UserIssuer)
                .Include(rt => rt.UserRecipent)
                .FirstOrDefaultAsync(rt => rt.Id == transferId);

            if (transferRequest == null)
            {
                throw new ArgumentException("Overførselsanmodning ikke fundet.");
            }
            return transferRequest;
        }

        public async Task<TransferRequest_ContractDTO> Get_RabbitTransfer_Contract(string userId, int transferId)
        {
            var transfer = await Get_TransferRequest(transferId);


            if (userId != transfer.IssuerId && userId != transfer.RecipentId)
            {
                throw new UnauthorizedAccessException("Du har ikke tilladelse til at se denne anmodning");
            }            

            if (transfer.Status == RequestStatus.Pending)
            {
                return new TransferRequest_ContractDTO
                {
                    Id = transfer.Id,
                    Status = transfer.Status,
                    Issuer_BreederRegNo = transfer.UserIssuer.BreederRegNo,

                    Issuer_FullName = $"{transfer.UserIssuer.FirstName} {transfer.UserIssuer.LastName}",
                    Issuer_Email = transfer.UserIssuer.Email,
                    Issuer_RoadNameAndNo = transfer.UserIssuer.RoadNameAndNo,
                    Issuer_ZipCode = transfer.UserIssuer.ZipCode,
                    Issuer_City = transfer.UserIssuer.City,                   

                    Recipent_BreederRegNo = transfer.UserRecipent.BreederRegNo,
                    DateAccepted = transfer.DateAccepted,

                    Rabbit_EarCombId = transfer.Rabbit.EarCombId,
                    Rabbit_NickName = transfer.Rabbit.NickName,
                    Price = transfer.Price,
                    SaleConditions = transfer.SaleConditions,
                };
            }
            else
            {
                return new TransferRequest_ContractDTO
                {
                    Id = transfer.Id,
                    Status = transfer.Status,
                    DateAccepted = transfer.DateAccepted,

                    Issuer_BreederRegNo = transfer.UserIssuer.BreederRegNo,

                    Issuer_FullName = $"{transfer.UserIssuer.FirstName} {transfer.UserIssuer.LastName}",
                    Issuer_Email = transfer.UserIssuer.Email,
                    Issuer_RoadNameAndNo = transfer.UserIssuer.RoadNameAndNo,
                    Issuer_ZipCode = transfer.UserIssuer.ZipCode,
                    Issuer_City = transfer.UserIssuer.City,

                    Recipent_BreederRegNo = transfer.UserRecipent.BreederRegNo,
                    Recipent_FullName = $"{transfer.UserRecipent.FirstName} {transfer.UserRecipent.LastName}",
                    Recipent_Email = transfer.UserRecipent.Email,
                    Recipent_RoadNameAndNo = transfer.UserRecipent.RoadNameAndNo,
                    Recipent_ZipCode = transfer.UserRecipent.ZipCode,
                    Recipent_City = transfer.UserRecipent.City,

                    Rabbit_EarCombId = transfer.Rabbit.EarCombId,
                    Rabbit_NickName = transfer.Rabbit.NickName,
                    Price = transfer.Price,
                    SaleConditions = transfer.SaleConditions,
                };
            }
        }

        //----------------------------------------------: DELETE :----------------------------------------------
        public async Task<TransferRequest_PreviewDTO> DeleteTransferRequest(string userId, int transferRequestId)
        {
            var transferRequest = await _dbRepository.GetObject_ByIntKEYAsync(transferRequestId);

            if (userId != transferRequest.IssuerId && userId != transferRequest.RecipentId)
            {
                throw new UnauthorizedAccessException("Du har ikke tilladelse til at slette denne anmodning.");
            }

            if (transferRequest.Status != RequestStatus.Pending)
            {
                throw new InvalidOperationException("Kun anmodninger i 'Pending' status kan slettes.");
            }

            await _dbRepository.DeleteObjectAsync(transferRequest);

            // Opretter og returnerer en DTO for det slettede objekt
            return new TransferRequest_PreviewDTO
            {
                Id = transferRequest.Id,
                Status = transferRequest.Status,
                DateAccepted = transferRequest.DateAccepted,

                Rabbit_EarCombId = transferRequest.RabbitId,
                Issuer_BreederRegNo = transferRequest.UserIssuer.BreederRegNo,
                Recipent_BreederRegNo = transferRequest.UserRecipent.BreederRegNo,

                Price = transferRequest.Price,
                SaleConditions = transferRequest.SaleConditions,
            };

        }

        //---------------------------------: HELPER METHODS :---------------------------------

    }
}
