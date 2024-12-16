using DB_AngoraLib.DTOs;
using DB_AngoraLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Services.BreederService
{
    public interface IBreederService
    {
        // ------------------------: GET
        Task<List<Breeder>> GetAll_Breeders();
        Task<Breeder?> Get_BreederById(string userId);
        Task<Breeder?> Get_BreederByBreederRegNo(string breederRegNo);

        //------------------------: ICOLLECTIONS
        Task<List<Rabbit_PreviewDTO>> GetAll_RabbitsOwned_Filtered(
          string userId, Rabbit_OwnedFilterDTO filter);
        Task<List<Rabbit_PreviewDTO>> GetAll_RabbitsLinked(
            string userId);

        Task<List<TransferRequest_ReceivedDTO>> GetAll_TransferRequests_Received(
            string userId, TransferRequest_ReceivedFilterDTO filter);
        Task<List<TransferRequest_SentDTO>> GetAll_TransferRequests_Sent(
            string userId, TransferRequest_SentFilterDTO filter);
    }
}
