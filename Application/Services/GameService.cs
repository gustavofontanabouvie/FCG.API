using Fcg.Application.DTOs.Game;
using Fcg.Application.Interfaces;
using Fcg.Data.Repositories.Interface;
using Fcg.Domain;
using Fcg.Domain.Common;
using Fcg.Shared;


namespace Fcg.Application.Services;

public class GameService : IGameService
{
    private readonly IUnityOfWork _unityOfWork;

    public GameService(IUnityOfWork unityOfWork)
    {
        _unityOfWork = unityOfWork;
    }

    public async Task<Result<CreateGameResponseDto>> CreateGame(CreateGameDto createGameDto, CancellationToken cancellationToken)
    {
        bool isGameRegistered = await _unityOfWork.Games.ExistsAsync(g => g.Name.Equals(createGameDto.name), cancellationToken);

        if (isGameRegistered)
        {
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
            return Result<ResponseSimpleGameDto>.Failure("Game is not registered");

        await _unityOfWork.Games.DeleteAsync(game);
        await _unityOfWork.CompleteAsync(cancellationToken);

        var responseDto = new ResponseSimpleGameDto(game.Id, game.Name);

        return Result<ResponseSimpleGameDto>.Success(responseDto);
    }

    public async Task<Result<IEnumerable<ResponseGameDto>>> GetAllGamesWithPromotion(CancellationToken cancellationToken)
    {
        var gamesWithPromo = await _unityOfWork.GamesCustom.GetAllGamesWithPromotion(cancellationToken);

        if (gamesWithPromo is null)
            return Result<IEnumerable<ResponseGameDto>>.Failure("No games with promotion");

        var responseDtos = gamesWithPromo.Select(game => new ResponseGameDto(game.Id, game.Name, game.Genre, game.ReleaseDate, game.Price, game.Promotions.Select(promo => new PromotionDto(promo.Id, promo.Name, promo.DiscountPercentage)).ToList()));

        return Result<IEnumerable<ResponseGameDto>>.Success(responseDtos);
    }

    public async Task<Result<ResponseGameDto>> GetGameById(int id, CancellationToken cancellationToken)
    {
        var game = await _unityOfWork.GamesCustom.GetGameById(id, cancellationToken);

        if (game is null)
            return Result<ResponseGameDto>.Failure("Game is not registered");

        ResponseGameDto gameDto = new ResponseGameDto(game.Id, game.Name, game.Genre, game.ReleaseDate, game.Price, game.Promotions.Select(promo => new PromotionDto(promo.Id, promo.Name, promo.DiscountPercentage)).ToList());

        return Result<ResponseGameDto>.Success(gameDto);
    }
}
