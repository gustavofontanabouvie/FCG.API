using Fcg.Application.DTOs.Game;
using Fcg.Application.DTOs.Promotion;
using Fcg.Application.Interfaces;
using Fcg.Application.Services;
using Fcg.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FCG.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionController : ControllerBase
    {
        private readonly IPromotionService _promotionService;

        public PromotionController(IPromotionService promotionService)
        {
            _promotionService = promotionService;
        }

        [SwaggerOperation(summary: "Find a promotion by ID", description: "Retrieves the data of a specific promotion based on their ID")]
        [SwaggerResponse(200, "Promotion found and returned", typeof(PromotionDto))]
        [SwaggerResponse(404, "Promotion with that ID was not found")]
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<PromotionDto>> GetPromotionById(int id, CancellationToken cancellationToken)
        {
            var result = await _promotionService.GetPromotionById(id, cancellationToken);

            if (!result.IsSuccess)
                return NotFound(new { error = result.Error });

            return Ok(result.Value);
        }

        [SwaggerOperation(summary: "Create a new promotion for a game", description: "Creates a new promotion and associates it with an existing game by its ID.")]
        [SwaggerResponse(201, "Promotion created successfully.", typeof(PromotionDto))]
        [SwaggerResponse(409, "A promotion with the same name already exists.")]
        [SwaggerResponse(422, "The specified game ID does not exist.")]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<PromotionDto>> PostPromotion([FromBody] CreatePromotionDto createPromotionDto, CancellationToken cancellationToken)

        {
            var result = await _promotionService.CreatePromotion(createPromotionDto, cancellationToken);

            if (!result.IsSuccess)
            {
                if (result.Error.Contains("already exists"))
                {
                    return Conflict(new { error = result.Error });
                }

                return UnprocessableEntity(new { error = result.Error });
            }

            return CreatedAtAction("GetPromotionById", new { id = result.Value.id }, result.Value);
        }


        [SwaggerOperation(summary: "Update an existing promotion", description: "Updates the details of an existing promotion by its ID.")]
        [SwaggerResponse(200, "Promotion updated successfully.", typeof(PromotionDto))]
        [SwaggerResponse(404, "Promotion with that ID was not found.")]
        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}")]
        public async Task<ActionResult<PromotionDto>> UpdatePromotion(int id, [FromBody] PromotionUpdateDto promoUpdateDto, CancellationToken cancellationToken)
        {
            var result = await _promotionService.UpdatePromotion(id, promoUpdateDto, cancellationToken);

            if (!result.IsSuccess)
                return NotFound(new { error = result.Error });

            return Ok(result.Value);
        }

        [SwaggerOperation(summary: "Delete a promotion by ID", description: "Deletes an existing promotion by its ID.")]
        [SwaggerResponse(204, "The promotion was deleted successfully.")]
        [SwaggerResponse(404, "Promotion with that ID was not found.")]
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePromotionById(int id, CancellationToken cancellationToken)
        {
            var result = await _promotionService.DeletePromotionById(id, cancellationToken);
            if (!result.IsSuccess)
                return NotFound(new { error = result.Error });

            return NoContent();
        }
    }
}
