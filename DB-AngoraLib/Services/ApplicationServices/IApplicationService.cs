using DB_AngoraLib.DTOs;
using DB_AngoraLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Services.ApplicationServices
{
    public interface IApplicationService
    {
        Task Apply_ApplicationBreeder(string userId, ApplicationBreeder_CreateDTO applicationDTO);
        Task Respond_ApplicationBreeder(int applicationId, ApplicationBreeder_ResponseDTO responseDTO);
        Task<IEnumerable<ApplicationBreeder>> GetAll_ApplicationBreeder_Pending();
    }
}
