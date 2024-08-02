using DB_AngoraLib.DTOs;
using DB_AngoraLib.Models;
using DB_AngoraLib.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Services.FavoriteService
{
    public class FavoriteServices
    {
        private readonly IGRepository<Favorite> _favoriteRepository;
        private readonly IGRepository<Rabbit> _rabbitRepository;
        private readonly IGRepository<Wool> _woolRepository;

        public FavoriteServices(
            IGRepository<Favorite> favoriteRepository,
            IGRepository<Rabbit> rabbitRepository,
            IGRepository<Wool> woolRepository)
        {
            _favoriteRepository = favoriteRepository;
            _rabbitRepository = rabbitRepository;
            _woolRepository = woolRepository;
        }

        public async Task AddFavoriteItem(string userId, string itemId, FavoriteType itemType)
        {
            var favorite = new Favorite
            {
                UserId = userId,
                ItemId = itemId,
                ItemType = itemType
            };

            await _favoriteRepository.AddObjectAsync(favorite);
        }

        public async Task RemoveFavoriteItem(string userId, string itemId, FavoriteType itemType)
        {
            var favorite = await _favoriteRepository
                .GetObject_ByFilterAsync(f => f.UserId == userId && f.ItemId == itemId && f.ItemType == itemType);

            if (favorite != null)
            {
                await _favoriteRepository.DeleteObjectAsync(favorite);
            }
        }

        public async Task<List<object>> GetAll_FavoriteItems(string userId)
        {
            var favorites = await _favoriteRepository
                .GetAllObjectsAsync();

            var favoriteItems = new List<object>();

            foreach (var favorite in favorites)
            {
                if (favorite.UserId == userId)
                {
                    if (favorite.ItemType == FavoriteType.Rabbit)
                    {
                        var rabbit = await _rabbitRepository.GetObject_ByStringKEYAsync(favorite.ItemId);
                        if (rabbit != null)
                        {
                            favoriteItems.Add(new Rabbit_PreviewDTO
                            {
                                EarCombId = rabbit.EarCombId,
                                NickName = rabbit.NickName,
                                Race = rabbit.Race,
                                Color = rabbit.Color,
                                Gender = rabbit.Gender,
                                UserOwner = rabbit.UserOwner?.UserName,
                                UserOrigin = rabbit.UserOrigin?.UserName
                            });
                        }
                    }
                    //else if (favorite.ItemType == FavoriteType.Wool)
                    //{
                    //    var wool = await _woolRepository.GetObject_ByStringKEYAsync(favorite.ItemId);
                    //    if (wool != null)
                    //    {
                    //        favoriteItems.Add(new WoolDTO
                    //        {
                    //            WoolType = wool.WoolType,
                    //            WoolColor = wool.WoolColor,
                    //            WoolQuality = wool.WoolQuality,
                    //            WoolLength = wool.WoolLength,
                    //            WoolWeight = wool.WoolWeight,
                    //            WoolDescription = wool.WoolDescription
                    //        });
                    //    }
                    //}
                }
            }

            return favoriteItems;
        }
    }
}
