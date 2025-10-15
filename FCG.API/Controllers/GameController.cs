using Fcg.Application.DTOs.Game;
using Fcg.Application.DTOs.User;
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

    [SwaggerOperation(summary: "Find a game by ID", Description = "Retrieves the data of a specific game based on their ID")]
    [SwaggerResponse(200, "Game found and returned", typeof(ResponseGameDto))]
    [SwaggerResponse(404, "Game with that ID was not found")]
    [HttpGet("{id}")]
    public async Task<ActionResult<ResponseGameDto>> GetGameById(int id, CancellationToken cancellationToken)
    {
        var result = await _gameService.GetGameById(id, cancellationToken);

        if (!result.IsSuccess)
            return NotFound(new { error = result.Error });

        return Ok(result.Value);
    }

    [SwaggerOperation(Summary = "Create a new Game", Description = "This endpoint receive a game data and persist in database")]
    [SwaggerResponse(201, "Game created successfuly", typeof(ResponseGameDto))]
    [SwaggerResponse(422, "That Game has already been registered")]
    [HttpPost]
    public async Task<ActionResult<ResponseGameDto>> PostGame([FromBody] CreateGameDto createGameDto, CancellationToken cancellationToken)
    {
        var result = await _gameService.CreateGame(createGameDto, cancellationToken);

        if (!result.IsSuccess)
            return UnprocessableEntity(new { error = result.Error });

        return CreatedAtAction("GetGameById", new { id = result.Value.id }, result.Value);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ResponseGameDto>>> GetAllGamesWithPromotion(CancellationToken cancellationToken)
    {
        var result = await _gameService.GetAllGamesWithPromotion(cancellationToken);

        if (!result.IsSuccess)
            return NotFound();

        return Ok(result.Value);
    }

}
