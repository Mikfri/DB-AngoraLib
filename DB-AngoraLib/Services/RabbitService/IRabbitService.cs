﻿using DB_AngoraLib.DTOs;
using DB_AngoraLib.Models;
using System.Security.Claims;

namespace DB_AngoraLib.Services.RabbitService
{
    public interface IRabbitService
    {
        Task<Rabbit_ProfileDTO> AddRabbit_ToMyCollection(string userId, Rabbit_CreateDTO newRabbit);

        Task<List<Rabbit>> Get_AllRabbits();
        Task<List<Rabbit_PreviewDTO>> Get_AllRabbits_ByBreederRegNo(string breederRegNo);
        Task<List<Rabbit_ForsalePreviewDTO>> Get_AllRabbits_Forsale_Filtered(Rabbit_ForsaleFilterDTO filter);
        Task<List<Rabbit_ForbreedingPreviewDTO>> Get_AllRabbits_Forbreeding_Filtered(Rabbit_ForbreedingFilterDTO filter);

        Task<Rabbit> Get_Rabbit_ByEarCombId(string earCombId);
        Task<Rabbit_ProfileDTO> Get_Rabbit_Profile(string userId, string earCombId, IList<Claim> userClaims);
        Task<Rabbit_ForsaleProfileDTO> Get_Rabbit_ForsaleProfile(string earCombId);
        Task<List<Rabbit_ChildPreviewDTO>> Get_Rabbit_ChildCollection(string earCombId);
        Task<Rabbit_PedigreeDTO> Get_RabbitPedigree(string earCombId/*, int maxGeneration*/);
        Task<Rabbit_PedigreeDTO> Get_RabbitTestParingPedigree(string fatherEarCombId, string motherEarCombId);

        Task<Rabbit_ProfileDTO> UpdateRabbit_RBAC(string userId, string earCombId, Rabbit_UpdateDTO updatedRabbit, IList<Claim> userClaims);
        Task UpdateRabbitOwnershipAsync(Rabbit rabbit);
        Task<Rabbit_PreviewDTO> DeleteRabbit_RBAC(string userId, string earCombId, IList<Claim> userClaims);

        Task LinkRabbits_ToNewBreederAsync(string userId, string breederRegNo);
    }
}