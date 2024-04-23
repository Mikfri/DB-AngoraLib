using DB_AngoraLib.Models;
using DB_AngoraLib.Repository;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;

public class PhotoService
{
    private readonly IGRepository<Photo> _photoRepository;

    public PhotoService(IGRepository<Photo> photoRepository)
    {
        _photoRepository = photoRepository;
    }

    public byte[] ConvertToJpeg(IFormFile file)
    {
        using var image = Image.Load(file.OpenReadStream());
        using var ms = new MemoryStream();
        image.SaveAsJpeg(ms);
        return ms.ToArray();
    }

    public async Task<Photo> CreatePhoto(IFormFile file, string entityId, string entityType)
    {
        var photo = new Photo
        {
            Image = ConvertToJpeg(file),
            FileName = file.FileName,
            ContentType = "image/jpeg",
            EntityId = entityId,
            EntityType = entityType
        };

        await _photoRepository.AddObjectAsync(photo);

        return photo;
    }
}
