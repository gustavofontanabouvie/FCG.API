using Fcg.Application.DTOs.Game;
using Fcg.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FCG.API.Controllers;


[Route("api/[controller]")]
[ApiController]
public class GameController : ControllerBase
{
    private readonly IGameService _gameService;

    public GameController(IGameService gameService)
    {
        _gameService = gameService;
    }

    [SwaggerOperation(Summary = "Create a new Game", Description = "This endpoint receive a game data and persist in database")]
    [SwaggerResponse(201, "Game created successfuly")]
    [SwaggerResponse(422, "That Game has already been registered")]
    [HttpPost]
    public async Task<ActionResult<ResponseGameDto>> PostGame(CreateGameDto createGameDto, CancellationToken cancellationToken)
    {
        var response = await _gameService.CreateGame(createGameDto, cancellationToken);

        if (response is null)
            return UnprocessableEntity();

        return CreatedAtAction("GetGame", new { name = response.name }, response);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ResponseGameDto>>> GetAllGamesWithPromotion(CancellationToken cancellationToken)
    {
        return await _gameService.GetAllGamesWithPromotion(cancellationToken);
    }

}
