using Fcg.Application.DTOs.Game;
using Fcg.Application.Interfaces;
using Fcg.Data.Repositories.Interface;
using Fcg.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Application.Services;

public class GameService : IGameService
{
    private readonly IGameRepository _gameRepository;

    public GameService(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }

    public async Task<ResponseGameDto> CreateGame(CreateGameDto createGameDto, CancellationToken cancellationToken)
    {
        var game = new Game
        {
            Name = createGameDto.name,
            Genre = createGameDto.genre,
            Price = createGameDto.price,
            ReleaseDate = createGameDto.releaseDate
        };

        bool isGameRegistered = _gameRepository.IsGameRegistered(game.Name, cancellationToken);

        if (!isGameRegistered)
        {
            var response = await _gameRepository.CreateGame(game, cancellationToken);

            var responseDto = new ResponseGameDto(response.Name, response.Genre, response.ReleaseDate, response.Price);

            return responseDto;
        }

        return null;
    }
}
