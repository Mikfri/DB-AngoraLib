﻿using DB_AngoraLib;
using DB_AngoraLib.DTOs;
using DB_AngoraLib.Models;
using DB_AngoraLib.Services.AccountService;
using DB_AngoraLib.Services.RabbitService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace DB_AngoraREST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RabbitController : ControllerBase
    {
        private readonly IRabbitService _rabbitService;

        public RabbitController(IRabbitService rabbitService)
        {
            _rabbitService = rabbitService;
        }

        //-------------------------------: POST

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpPost("Create")]    // FromBody er bedst til SPA applikationer
        [Authorize(Roles = "Admin, Moderator, BreederPremium, BreederBasic")]
        public async Task<ActionResult<Rabbit_ProfileDTO>> AddRabbit([FromBody] Rabbit_CreateDTO newRabbitDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                // Pass the userId and newRabbitDto to your service method
                var createdRabbit = await _rabbitService.AddRabbit_ToMyCollection(userId, newRabbitDto);

                // Use CreatedAtAction with GetRabbit_ProfileByEarTags
                return CreatedAtAction(nameof(GetRabbit_ProfileByEarTags), new { earCombId = createdRabbit.EarCombId }, createdRabbit);
            }
            catch (InvalidOperationException ex)    // ved at have catch på vil RabbitService fejl beskeden kunne sendes tilbage til klienten
            {
                // Log the error here if needed
                return BadRequest(new { message = ex.Message });
            }
        }

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpPost("Create_Form")]
        [Authorize(Roles = "Admin, Moderator, BreederPremium, BreederBasic")]
        public async Task<ActionResult<Rabbit_ProfileDTO>> AddRabbit_Form([FromForm] Rabbit_CreateDTO newRabbitDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                // Pass the userId and newRabbitDto to your service method
                var createdRabbit = await _rabbitService.AddRabbit_ToMyCollection(userId, newRabbitDto);

                // Use CreatedAtAction with GetRabbit_ProfileByEarTags
                return CreatedAtAction(nameof(GetRabbit_ProfileByEarTags), new { earCombId = createdRabbit.EarCombId }, createdRabbit);
            }
            catch (InvalidOperationException ex)    // ved at have catch på vil RabbitService fejl beskeden kunne sendes tilbage til klienten
            {
                // Log the error here if needed
                return BadRequest(new { message = ex.Message });
            }
        }

        //-------------------------------: GET

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "Admin")]
        [HttpGet("All")]
        public async Task<ActionResult<IEnumerable<Rabbit>>> GetAllRabbits()
        {
            var rabbits = await _rabbitService.Get_AllRabbits();
            return Ok(rabbits);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpGet("Profile/{earCombId}")]
        //[Authorize(Roles = "Admin, Moderator, BreederPremium, BreederBasic")]
        public async Task<ActionResult<Rabbit_ProfileDTO>> GetRabbit_ProfileByEarTags(string earCombId)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userClaims = User.Claims.ToList();
            var rabbitProfile = await _rabbitService.Get_Rabbit_Profile(currentUserId, earCombId, userClaims);

            if (rabbitProfile == null)
            {
                return NotFound();
            }

            return Ok(rabbitProfile);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("ForsaleProfile/{earCombId}")]
        public async Task<ActionResult<Rabbit_ForsaleProfileDTO>> GetRabbit_ForsaleProfileByEarTags(string earCombId)
        {
            var rabbitForsaleProfile = await _rabbitService.Get_Rabbit_ForsaleProfile(earCombId);

            if (rabbitForsaleProfile == null)
            {
                return NotFound(new { message = $"Rabbit with EarCombId '{earCombId}' not found or not for sale." });
            }

            return Ok(rabbitForsaleProfile);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpGet("Pedigree/{earCombId}")]
        [Authorize(Roles = "Admin, Moderator, BreederPremium, BreederBasic")] // Juster adgangskontrollen efter behov
        public async Task<IActionResult> GetRabbitPedigree(string earCombId)
        {
            try
            {
                var rabbitPedigree = await _rabbitService.Get_RabbitPedigree(earCombId);

                if (rabbitPedigree == null)
                {
                    return NotFound(new { message = $"Pedigree for rabbit with EarCombId '{earCombId}' not found." });
                }

                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                    WriteIndented = true
                };
                options.Converters.Add(new JsonStringEnumConverter());

                var json = JsonSerializer.Serialize(rabbitPedigree, options);
                return new ContentResult
                {
                    Content = json,
                    ContentType = "application/json",
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception ex) // Overvej at fange mere specifikke undtagelser, hvis det er muligt
            {
                // Log fejlen her, hvis nødvendigt
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpGet("TestParingPedigree")]
        [Authorize(Roles = "Admin, Moderator, BreederPremium, BreederBasic")]
        public async Task<IActionResult> GetRabbitTestParingPedigree(
            [FromQuery] string fatherEarCombId,
            [FromQuery] string motherEarCombId)
        {
            try
            {
                var pedigree = await _rabbitService.Get_RabbitTestParingPedigree(fatherEarCombId, motherEarCombId);

                if (pedigree == null)
                {
                    return NotFound(new { message = "Pedigree could not be generated for the provided ear comb IDs." });
                }

                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                    WriteIndented = true
                };
                options.Converters.Add(new JsonStringEnumConverter());

                var json = JsonSerializer.Serialize(pedigree, options);
                return new ContentResult
                {
                    Content = json,
                    ContentType = "application/json",
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the error here if needed
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }



        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpGet("ByBreeder/{breederRegNo}")]
        [Authorize(Roles = "Admin, Moderator")]
        public async Task<ActionResult<IEnumerable<Rabbit>>> GetRabbitsByBreeder(string breederRegNo)
        {
            var rabbits = await _rabbitService.Get_AllRabbits_ByBreederRegNo(breederRegNo);
            if (rabbits == null || !rabbits.Any())
            {
                return NotFound();
            }
            return Ok(rabbits);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpGet("Forsale")]
        public async Task<ActionResult<List<Rabbit_ForsalePreviewDTO>>> GetFilteredRabbitsForSale([FromQuery] Rabbit_ForsaleFilterDTO filter)
        {
            var rabbits = await _rabbitService.Get_AllRabbits_Forsale_Filtered(filter);
            return Ok(rabbits);
        }


        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("Forbreeding")]
        public async Task<ActionResult<List<Rabbit_ForbreedingPreviewDTO>>> GetAllRabbits_ForbreedingFiltered([FromQuery] Rabbit_ForbreedingFilterDTO filter)
        {
            var rabbits = await _rabbitService.Get_AllRabbits_Forbreeding_Filtered(filter);
            return Ok(rabbits);
        }


        //-------------------------------: PUT
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Policy = "UpdateRabbit")]
        [HttpPut("Update/{earCombId}")]
        public async Task<ActionResult<Rabbit_ProfileDTO>> UpdateRabbit(string earCombId, [FromBody] Rabbit_UpdateDTO updatedRabbit)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userClaims = User.Claims.ToList();

            try
            {
                var updatedRabbitDTO = await _rabbitService.UpdateRabbit_RBAC(userId, earCombId, updatedRabbit, userClaims);
                if (updatedRabbitDTO == null)
                {
                    return NotFound();
                }
                return Ok(updatedRabbitDTO);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //-------------------------------: DELETE

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Policy = "DeleteRabbit")]
        [HttpDelete("Delete/{earCombId}")]
        public async Task<ActionResult<Rabbit_PreviewDTO>> DeleteRabbit(string earCombId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userClaims = User.Claims.ToList();

            try
            {
                var rabbitPreviewDTO = await _rabbitService.DeleteRabbit_RBAC(userId, earCombId, userClaims);
                return Ok(rabbitPreviewDTO);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }



    }
}
