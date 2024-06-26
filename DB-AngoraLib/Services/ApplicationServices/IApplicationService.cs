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
        Task ApplyForBreederRoleAsync(string userId, Application_BreederDTO applicationDTO);
        Task ApproveApplicationAsync(int applicationId);
        Task RejectApplicationAsync(int applicationId, string rejectionReason);

        Task<IEnumerable<BreederApplication>> GetPendingApplicationsAsync();
    }
}
