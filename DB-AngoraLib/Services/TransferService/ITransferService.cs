using DB_AngoraLib.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Services.TransferService
{
    public interface ITransferService
    {
        Task<TransferRequest_PreviewDTO> CreateTransferRequest(string issuerId, TransferRequest_CreateDTO createTransferDTO);
        Task<TransferRequest_ContractDTO> Get_RabbitTransfer_Contract(string userId, int transferId);
        Task<TransferRequest_ContractDTO> Response_TransferRequest(string recipentId, int transferId, TransferRequest_ResponseDTO responseDTO);
    }
}
