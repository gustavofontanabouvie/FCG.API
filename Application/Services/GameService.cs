using Fcg.Application.DTOs.Game;
using Fcg.Application.Interfaces;
using Fcg.Data.Repositories.Interface;
using Fcg.Domain;
using Fcg.Domain.Common;
using Fcg.Shared;
using Microsoft.Extensions.Logging;


namespace Fcg.Application.Services;

public class GameService : IGameService
{
    private readonly IUnityOfWork _unityOfWork;
    private readonly ILogger<GameService> _logger;

    public GameService(IUnityOfWork unityOfWork)
    {
        _unityOfWork = unityOfWork;
    }

    public GameService(IUnityOfWork unityOfWork, ILogger<GameService> logger)
    {
        _unityOfWork = unityOfWork;
        _logger = logger;
    }

    public async Task<Result<CreateGameResponseDto>> CreateGame(CreateGameDto createGameDto, CancellationToken cancellationToken)
    {

        bool isGameRegistered = await _unityOfWork.Games.ExistsAsync(g => g.Name.Equals(createGameDto.name), cancellationToken);

        if (isGameRegistered)
        {
            _logger.LogWarning("A game with this name already exists");
            return Result<CreateGameResponseDto>.Failure("A game with this name already exists");
        }

        var game = new Game
        {
            Name = createGameDto.name,
            Genre = createGameDto.genre,
            Price = createGameDto.price,
            ReleaseDate = createGameDto.releaseDate
        };

        var response = await _unityOfWork.GamesCustom.CreateGame(game, cancellationToken);

        var responseDto = new CreateGameResponseDto(response.Id, response.Name, response.Genre, response.ReleaseDate, response.Price);

        return Result<CreateGameResponseDto>.Success(responseDto);
    }

    public async Task<Result<ResponseSimpleGameDto>> DeleteGameById(int id, CancellationToken cancellationToken)
    {
        var game = await _unityOfWork.Games.GetByIdAsync(id, cancellationToken);

        if (game is null)
        {
            _logger.LogWarning("Game is not registered");
            return Result<ResponseSimpleGameDto>.Failure("Game is not registered");
        }

        await _unityOfWork.Games.DeleteAsync(game, cancellationToken);
        await _unityOfWork.CompleteAsync(cancellationToken);

        var responseDto = new ResponseSimpleGameDto(game.Id, game.Name);

        return Result<ResponseSimpleGameDto>.Success(responseDto);
    }

    public async Task<Result<IEnumerable<ResponseGameDto>>> GetAllGamesWithPromotion(CancellationToken cancellationToken)
    {
        var gamesWithPromo = await _unityOfWork.GamesCustom.GetAllGamesWithPromotion(cancellationToken);

        if (gamesWithPromo is null)
        {
            _logger.LogWarning("No games with promotion");
            return Result<IEnumerable<ResponseGameDto>>.Failure("No games with promotion");
        }

        var responseDtos = gamesWithPromo.Select(game => new ResponseGameDto(game.Id, game.Name, game.Genre, game.ReleaseDate, game.Price, game.Promotions.Select(promo => new PromotionDto(promo.Id, promo.Name, promo.DiscountPercentage)).ToList()));

        return Result<IEnumerable<ResponseGameDto>>.Success(responseDtos);
    }

    public async Task<Result<ResponseGameDto>> GetGameById(int id, CancellationToken cancellationToken)
    {
        var game = await _unityOfWork.GamesCustom.GetGameById(id, cancellationToken);

        if (game is null)
        {
            _logger.LogWarning("Game is not registered");
            return Result<ResponseGameDto>.Failure("Game is not registered");
        }

        ResponseGameDto gameDto = new ResponseGameDto(game.Id, game.Name, game.Genre, game.ReleaseDate, game.Price, game.Promotions.Select(promo => new PromotionDto(promo.Id, promo.Name, promo.DiscountPercentage)).ToList());

        return Result<ResponseGameDto>.Success(gameDto);
    }

    public async Task<Result<ResponseGameDto>> UpdateGameById(int id, UpdateGameDto updateGameDto, CancellationToken cancellationToken)
    {
        var game = await _unityOfWork.GamesCustom.GetGameByIdUpdate(id, cancellationToken);

        if (game is null)
        {
            _logger.LogWarning("Game is not registered");
            return Result<ResponseGameDto>.Failure("Game is not registered");
        }

        if (updateGameDto.name is not null)
            game.Name = updateGameDto.name;

        if (updateGameDto.genre is not null)
            game.Genre = updateGameDto.genre;

        if (updateGameDto.releaseDate is not null)
            game.ReleaseDate = updateGameDto.releaseDate.Value;

        if (updateGameDto.price is not null)
            game.Price = updateGameDto.price.Value;

        await _unityOfWork.Games.UpdateAsync(game, cancellationToken);
        await _unityOfWork.CompleteAsync(cancellationToken);

        ResponseGameDto gameDto = new ResponseGameDto(game.Id, game.Name, game.Genre, game.ReleaseDate, game.Price, game.Promotions.Select(promo => new PromotionDto(promo.Id, promo.Name, promo.DiscountPercentage)).ToList());

        return Result<ResponseGameDto>.Success(gameDto);
    }
}
