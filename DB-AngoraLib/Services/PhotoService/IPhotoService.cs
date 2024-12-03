using DB_AngoraLib.DTOs;
using DB_AngoraLib.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Services.PhotoService
{
    public interface IPhotoService
    {
        Task<Photo_DTO> UploadPhotoAsync(IFormFile file, string? rabbitId = null, string? userId = null);
        Task SetAsProfilePhotoAsync(int photoId, string? rabbitId = null, string? userId = null);
        Task DeletePhotoAsync(int photoId);

    }
}
