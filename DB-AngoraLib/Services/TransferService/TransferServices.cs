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


        public TransferServices(IGRepository<RabbitTransfer> dbRepository, IRabbitService rabbitService ,IAccountService userService)
        {
            _dbRepository = dbRepository;
            _rabbitServices = rabbitService;
            _accountService = userService;
        }

        public async Task<RabbitTransfer_PreviewDTO> CreateTransferRequestAsync(string userId, RabbitTransfer_CreateDTO transferDTO)
        {
            // Valider input (Dette trin kan udvides med mere detaljerede valideringer)
            if (transferDTO == null)
                throw new ArgumentNullException(nameof(transferDTO));

            // Tjek for eksisterende kanin
            var rabbit = await _rabbitServices.Get_Rabbit_ByEarCombId(transferDTO.EarCombId) 
                ?? throw new ArgumentException("Kaninen blev ikke fundet");

            // Tjek for eksisterende nuværende ejer
            var currentOwner = await _accountService.Get_UserByIdAsync(userId)
                ?? throw new ArgumentException("Nuværende ejer blev ikke fundet.");

            if (rabbit.OwnerId != userId)
                throw new InvalidOperationException("Du er ikke den nuværende ejer af denne kanin.");

            var proposedOwner = await _accountService.Get_UserByBreederRegNoAsync(transferDTO.ProposedOwnerBreederRegNo)
                ?? throw new ArgumentException("Foreslået ejer blev ikke fundet.");

            // Tjek for eksisterende aktive RabbitTransfer anmodninger for den samme kanin
            var existingTransfers = await _dbRepository.GetDbSet()
                .Where(rt => rt.RabbitId == rabbit.EarCombId && rt.Status == RequestStatus.Pending)
                .ToListAsync();

            if (existingTransfers.Any())
            {
                throw new InvalidOperationException("Der findes allerede en aktiv overførselsanmodning for denne kanin.");
            }


            // Opret overførselsanmodning
            var transferRequest = new RabbitTransfer
            {
                Status = RequestStatus.Pending,
                RequestDate = DateTime.Now,
                ExpirationDate = DateTime.Now.AddDays(7), // Antager 7 dages udløb

                CurrentOwnerId = userId,
                RabbitId = rabbit.EarCombId,
                ProposedOwnerId = proposedOwner.Id,
                //ResponseDate = null,
            };

            await _dbRepository.AddObjectAsync(transferRequest);

            // Returner en bekræftelse
            return new RabbitTransfer_PreviewDTO
            {
                Id = transferRequest.Id,
                Status = transferRequest.Status,
                RequestDate = transferRequest.RequestDate,
                ExpirationDate = transferRequest.ExpirationDate,

                CurrentOwnerId = transferRequest.CurrentOwnerId,
                RabbitId = transferRequest.RabbitId,
                ProposedOwnerId = transferRequest.ProposedOwnerId,

                ResponseDate = transferRequest.ResponseDate, // Vil være null, da dette er en ny anmodning
            };
        }

        public async Task<RabbitTransfer_PreviewDTO> RespondToTransferRequestAsync(string respondingUserId, int transferId, bool accept)
        {
            var transfer = await _dbRepository.GetDbSet()
                .Include(rt => rt.RabbitInTrans)
                .FirstOrDefaultAsync(rt => rt.Id == transferId);

            if (transfer == null)
            {
                throw new ArgumentException("Overførselsanmodning ikke fundet.");
            }

            if (transfer.Status != RequestStatus.Pending)
            {
                throw new InvalidOperationException("Denne anmodning er ikke længere aktiv.");
            }

            if (transfer.ProposedOwnerId != respondingUserId)
            {
                throw new InvalidOperationException("Du har ikke tilladelse til at svare på denne anmodning.");
            }

            if (transfer.ExpirationDate < DateTime.Now)
            {
                throw new InvalidOperationException("Denne anmodning er udløbet.");
            }

            transfer.Status = accept ? RequestStatus.Approved : RequestStatus.Rejected;
            transfer.ResponseDate = DateTime.Now;

            if (accept)
            {
                transfer.RabbitInTrans.OwnerId = transfer.ProposedOwnerId;
                // Opdater kaninen i databasen
                await _rabbitServices.UpdateRabbitOwnershipAsync(transfer.RabbitInTrans);
            }

            await _dbRepository.UpdateObjectAsync(transfer);

            return new RabbitTransfer_PreviewDTO
            {
                Id = transfer.Id,
                Status = transfer.Status,
                RequestDate = transfer.RequestDate,
                ExpirationDate = transfer.ExpirationDate,
                CurrentOwnerId = transfer.CurrentOwnerId,
                RabbitId = transfer.RabbitId,
                ProposedOwnerId = transfer.ProposedOwnerId,
                ResponseDate = transfer.ResponseDate
            };
        }

        public async Task HandleExpiredTransfersAsync()
        {
            var expiredTransfers = await _dbRepository.GetDbSet()
                .Where(rt => rt.Status == RequestStatus.Pending && rt.ExpirationDate < DateTime.Now)
                .ToListAsync();

            foreach (var transfer in expiredTransfers)
            {
                transfer.Status = RequestStatus.Expired;
            }

            await _dbRepository.UpdateObjectsListAsync(expiredTransfers);
        }

    }
}
