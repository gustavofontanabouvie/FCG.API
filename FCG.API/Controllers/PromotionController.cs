using Fcg.Application.DTOs.Game;
using Fcg.Application.DTOs.Promotion;
using Fcg.Application.Interfaces;
using Fcg.Application.Services;
using Fcg.Shared;
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


    }
}
