using DB_AngoraLib.EF_DbContext;
using DB_AngoraLib.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Services.SaleDetailsService
{
    public class SaleDetailsServices
    {
        //private readonly DB_AngoraContext _context;

        //public SaleDetailsServices(DB_AngoraContext context)
        //{
        //    _context = context;
        //}

        //public async Task<List<Rabbit_ForsalePreviewDTO>> GetAllRabbitsForSaleAsync()
        //{
        //    var rabbitsForSale = await _context.SaleDetails
        //        .Where(sd => sd.RabbitId != null)
        //        .Include(sd => sd.Rabbit)
        //            .ThenInclude(r => r.UserOwner)
        //        .Include(sd => sd.Rabbit.Photos)
        //        .Select(sd => new Rabbit_ForSaleDTO
        //        {
        //            EarCombId = sd.Rabbit.EarCombId,
        //            NickName = sd.Rabbit.NickName,
        //            Race = sd.Rabbit.Race,
        //            Color = sd.Rabbit.Color,
        //            DateOfBirth = sd.Rabbit.DateOfBirth,
        //            Gender = sd.Rabbit.Gender,
        //            OwnerName = $"{sd.Rabbit.UserOwner.FirstName} {sd.Rabbit.UserOwner.LastName}",
        //            Price = sd.Price,
        //            SaleDescription = sd.SaleDescription,
        //            DateListed = sd.DatePublished,
        //            Photos = sd.Rabbit.Photos.Select(p => p.FilePath).ToList()
        //        })
        //        .ToListAsync();

        //    return rabbitsForSale;
        //}

        //public async Task<Rabbit_ForsalePreviewDTO> GetRabbitForSaleAsync(string earCombId)
        //{
        //    var saleDetails = await _context.SaleDetails
        //        .Include(sd => sd.Rabbit)
        //            .ThenInclude(r => r.UserOwner)
        //        .Include(sd => sd.Rabbit.Photos)
        //        .FirstOrDefaultAsync(sd => sd.RabbitId == earCombId);

        //    if (saleDetails == null)
        //    {
        //        return null;
        //    }

        //    var dto = new Rabbit_ForsalePreviewDTO
        //    {
        //        EarCombId = saleDetails.Rabbit.EarCombId,
        //        NickName = saleDetails.Rabbit.NickName,
        //        Race = saleDetails.Rabbit.Race,
        //        Color = saleDetails.Rabbit.Color,
        //        DateOfBirth = saleDetails.Rabbit.DateOfBirth,
        //        Gender = saleDetails.Rabbit.Gender,
        //        OwnerName = $"{saleDetails.Rabbit.UserOwner.FirstName} {saleDetails.Rabbit.UserOwner.LastName}",
        //        Price = saleDetails.Price,
        //        SaleDescription = saleDetails.SaleDescription,
        //        DateListed = saleDetails.DatePublished,
        //        Photos = saleDetails.Rabbit.Photos.Select(p => p.FilePath).ToList()
        //    };

        //    return dto;
        //}
    }
}
