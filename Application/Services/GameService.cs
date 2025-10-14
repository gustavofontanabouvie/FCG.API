using Fcg.Application.DTOs.Game;
using Fcg.Application.Interfaces;
using Fcg.Data.Repositories.Interface;
using Fcg.Domain;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Application.Services;

public class GameService : IGameService
{
    private readonly IUnityOfWork _unityOfWork;

    public GameService(IUnityOfWork unityOfWork)
    {
        _unityOfWork = unityOfWork;
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

        bool isGameRegistered = _unityOfWork.GamesCustom.IsGameRegistered(game.Name, cancellationToken);

        if (!isGameRegistered)
        {
            var response = await _unityOfWork.GamesCustom.CreateGame(game, cancellationToken);

            var responseDto = new ResponseGameDto(response.Name, response.Genre, response.ReleaseDate, response.Price);

            return responseDto;
        }

        return null;
    }

    public async Task<ActionResult<IEnumerable<ResponseGameDto>>> GetAllGamesWithPromotion(CancellationToken cancellationToken)
    {
        var response = await _unityOfWork.GamesCustom.GetAllGamesWithPromotion(cancellationToken);

        //return response;
        return null;
    }
}
