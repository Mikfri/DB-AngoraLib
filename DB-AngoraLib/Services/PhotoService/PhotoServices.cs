using DB_AngoraLib.DTOs;
using DB_AngoraLib.EF_DbContext;
using DB_AngoraLib.Models;
using DB_AngoraLib.Repository;
using DB_AngoraLib.Services.PhotoService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using System.ComponentModel.DataAnnotations;
using System.Linq;

public class PhotoService : IPhotoService
{
    private readonly IGRepository<Photo> _photoRepository;
    private readonly IGRepository<Rabbit> _rabbitRepository;
    private readonly IGRepository<User> _userRepository;
    private readonly IWebHostEnvironment _environment;

    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
    private const int MAX_FILE_SIZE = 5 * 1024 * 1024; // 5MB


    public PhotoService(
        IGRepository<Photo> photoRepository,
        IGRepository<Rabbit> rabbitRepository,
        IGRepository<User> userRepository,
        IWebHostEnvironment environment)
    {
        _photoRepository = photoRepository;
        _rabbitRepository = rabbitRepository;
        _userRepository = userRepository;
        _environment = environment;
    }

    //--------------------: CREATE
    public async Task<Photo_DTO> UploadPhotoAsync(IFormFile file, string? rabbitId = null, string? userId = null)
    {
        await ValidatePhotoAsync(file);

        var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName).ToLowerInvariant()}";
        var uploadsPath = Path.Combine(_environment.WebRootPath, "images");
        var filePath = Path.Combine(uploadsPath, uniqueFileName);

        Directory.CreateDirectory(uploadsPath);

        // Brug FileStream med async operations
        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var photo = new Photo
        {
            FilePath = $"/images/{uniqueFileName}",
            FileName = file.FileName,
            UploadDate = DateTime.UtcNow,
            RabbitId = rabbitId,
            UserId = userId
        };

        await _photoRepository.AddObjectAsync(photo);

        return MapToPhotoDTO(photo);
    }

    private async Task ValidatePhotoAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ValidationException("Ingen fil blev uploadet");

        if (file.Length > MAX_FILE_SIZE)
            throw new ValidationException($"Filen er for stor. Maximum størrelse er {MAX_FILE_SIZE / 1024 / 1024}MB");

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!_allowedExtensions.Contains(extension))
            throw new ValidationException($"Ugyldig filtype. Kun {string.Join(", ", _allowedExtensions)} er tilladt");

        // Valider at filen faktisk er et billede
        try
        {
            await using var stream = file.OpenReadStream();
            using var image = await Image.LoadAsync(stream);
        }
        catch (Exception)
        {
            throw new ValidationException("Filen er ikke et gyldigt billedformat");
        }
    }

    private Photo_DTO MapToPhotoDTO(Photo photo) =>
        new()
        {
            Id = photo.Id,
            FilePath = photo.FilePath,
            FileName = photo.FileName,
            UploadDate = photo.UploadDate,
            RabbitId = photo.RabbitId,
            UserId = photo.UserId,
            IsProfilePicture = photo.IsProfilePicture
        };

    public async Task SetAsProfilePhotoAsync(int photoId, string? rabbitId = null, string? userId = null)
    {
        var photo = await _photoRepository.GetObject_ByIntKEYAsync(photoId);
        if (photo == null)
            throw new InvalidOperationException("Billede ikke fundet");

        if (rabbitId is not null)
        {
            var rabbit = await _rabbitRepository.GetObject_ByStringKEYAsync(rabbitId);
            if (rabbit == null)
                throw new InvalidOperationException("Kanin ikke fundet");

            // Nulstil IsProfilePicture for alle andre fotos tilknyttet kaninen
            var rabbitPhotos = await _photoRepository.GetAllObjectsAsync();
            var photosToUpdate = rabbitPhotos.Where(p => p.RabbitId == rabbitId && p.Id != photoId && p.IsProfilePicture);
            foreach (var p in photosToUpdate)
            {
                p.IsProfilePicture = false;
                await _photoRepository.UpdateObjectAsync(p);
            }

            // Sæt IsProfilePicture til true for det valgte foto
            photo.IsProfilePicture = true;
            await _photoRepository.UpdateObjectAsync(photo);

            // Opdater kaninens ProfilePic
            rabbit.ProfilePicture = photo.FilePath;
            await _rabbitRepository.UpdateObjectAsync(rabbit);
        }
        else if (userId is not null)
        {
            var user = await _userRepository.GetObject_ByStringKEYAsync(userId);
            if (user == null)
                throw new InvalidOperationException("Bruger ikke fundet");

            // Nulstil IsProfilePicture for alle andre fotos tilknyttet brugeren
            var userPhotos = await _photoRepository.GetAllObjectsAsync();
            var photosToUpdate = userPhotos.Where(p => p.UserId == userId && p.Id != photoId && p.IsProfilePicture);
            foreach (var p in photosToUpdate)
            {
                p.IsProfilePicture = false;
                await _photoRepository.UpdateObjectAsync(p);
            }

            // Sæt IsProfilePicture til true for det valgte foto
            photo.IsProfilePicture = true;
            await _photoRepository.UpdateObjectAsync(photo);

            // Opdater brugerens ProfilePic
            user.ProfilePicture = photo.FilePath;
            await _userRepository.UpdateObjectAsync(user);
        }
        else
        {
            throw new ArgumentException("Enten rabbitId eller userId skal angives");
        }
    }

    public async Task DeletePhotoAsync(int photoId)
    {
        var photo = await _photoRepository.GetObject_ByIntKEYAsync(photoId);
        if (photo == null)
            throw new InvalidOperationException("Billede ikke fundet");

        // Slet fil fra filsystem
        var filePath = Path.Combine(_environment.WebRootPath, photo.FilePath.TrimStart('/'));
        if (File.Exists(filePath))
            File.Delete(filePath);

        await _photoRepository.DeleteObjectAsync(photo);
    }
}