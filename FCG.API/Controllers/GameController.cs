using Fcg.Application.DTOs.Game;
using Fcg.Application.DTOs.User;
using Fcg.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public async Task<ActionResult<ResponseGameDto>> GetGameById(int id, CancellationToken cancellationToken)
    {
        var result = await _gameService.GetGameById(id, cancellationToken);

        if (!result.IsSuccess)
            return NotFound(new { error = result.Error });

        return Ok(result.Value);
    }

    [SwaggerOperation(Summary = "Create a new Game", Description = "This endpoint receive a game data and persist in database")]
    [SwaggerResponse(201, "Game created successfuly", typeof(ResponseGameDto))]
    [SwaggerResponse(409, "That Game has already been registered")]
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ResponseGameDto>> PostGame([FromBody] CreateGameDto createGameDto, CancellationToken cancellationToken)
    {
        var result = await _gameService.CreateGame(createGameDto, cancellationToken);

        if (!result.IsSuccess)
            return Conflict(new { error = result.Error });

        return CreatedAtAction("GetGameById", new { id = result.Value.id }, result.Value);
    }


    [SwaggerOperation(Summary = "Get all games with active promotions", Description = "Retrieves a list of all games that currently have one or more active promotions")]
    [SwaggerResponse(200, "A list of games with promotions was returned successfully", typeof(IEnumerable<ResponseGameDto>))]
    [SwaggerResponse(204, "No games with promotions were found")]
    [HttpGet("with-promotion")]
    public async Task<ActionResult<IEnumerable<ResponseGameDto>>> GetAllGamesWithPromotion(CancellationToken cancellationToken)
    {
        var result = await _gameService.GetAllGamesWithPromotion(cancellationToken);

        if (!result.Value.Any())
            return NoContent();

        return Ok(result.Value);
    }


    [SwaggerOperation(Summary = "Delete a game by ID", Description = "Delete the data of a game and yours promotions")]
    [SwaggerResponse(404, "Game with that ID was not found")]
    [SwaggerResponse(204, "The game was deleted with success")]
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGameById(int id, CancellationToken cancellationToken)
    {
        var result = await _gameService.DeleteGameById(id, cancellationToken);

        if (!result.IsSuccess)
            return NotFound(new { error = result.Error });

        return NoContent();
    }

    [SwaggerOperation(Summary = "Update a game by ID", Description = "Update the data of a specific game based on their ID")]
    [SwaggerResponse(204, "Game updated successfully", typeof(ResponseGameDto))]
    [SwaggerResponse(404, "Game with that ID was not found")]
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<ResponseGameDto>> UpdateGameById(int id, [FromBody] UpdateGameDto updateGameDto, CancellationToken cancellationToken)
    {
        var result = await _gameService.UpdateGameById(id, updateGameDto, cancellationToken);
        if (!result.IsSuccess)
            return NotFound(new { error = result.Error });

        return NoContent();
    }

}
