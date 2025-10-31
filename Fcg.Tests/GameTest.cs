using Fcg.Application.DTOs.Game;
using Fcg.Application.DTOs.User;
using Fcg.Application.Interfaces;
using Fcg.Application.Services;
using Fcg.Data.Repositories.Implementation;
using Fcg.Data.Repositories.Interface;
using Fcg.Domain;
using Fcg.Domain.Common;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;


namespace Fcg.Tests;

public class GameTest
{
    private readonly Mock<IUnityOfWork> _unitOfWorkMock;
    private readonly Mock<IRepository<Game>> _gameRepoMock;
    private readonly Mock<IGameRepository> _gamesCustomRepoMock;
    private readonly Mock<ILogger<GameService>> _loggerMock;

    private readonly GameService _gameService;

    public GameTest()
    {
        _unitOfWorkMock = new Mock<IUnityOfWork>();
        _gameRepoMock = new Mock<IRepository<Game>>();
        _gamesCustomRepoMock = new Mock<IGameRepository>();
        _loggerMock = new Mock<ILogger<GameService>>();

        _unitOfWorkMock.Setup(u => u.Games).Returns(_gameRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.GamesCustom).Returns(_gamesCustomRepoMock.Object);


        _gameService = new GameService(_unitOfWorkMock.Object, _loggerMock.Object);
    }
    [Fact]
    public async Task CreateGame_WithValidData_ShouldCreateGameSuccessfully()
    {
        // Arrange
        var newGameDto = new CreateGameDto("Game Test", "Action", new DateTime(2023, 1, 1), 50.22m);
        var expectedCreatedGame = new Game { Id = 1, Name = newGameDto.name };

        _gameRepoMock.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Game, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _gamesCustomRepoMock
            .Setup(r => r.CreateGame(It.IsAny<Game>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedCreatedGame);


        // Act
        var result = await _gameService.CreateGame(newGameDto, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();

        _gamesCustomRepoMock.Verify(r => r.CreateGame(It.Is<Game>(g =>
            g.Name == newGameDto.name), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateGame_WithNameInUse_ShouldFailure()
    {
        // Arrange
        var newGameDto = new CreateGameDto("Game Test", "Action", new DateTime(2023, 1, 1), 50.22m);

        _gameRepoMock.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Game, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _unitOfWorkMock.Setup(u => u.Games).Returns(_gameRepoMock.Object);

        // Act
        var result = await _gameService.CreateGame(newGameDto, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeNull();
        result.Error.Should().Be("A game with this name already exists");

    }

    [Fact]
    public async Task DeleteGameById_WithValidId_ShouldDeleteGameSuccessfully()
    {
        // Arrange
        int gameId = 1;
        var gameToDelete = new Game { Id = gameId, Name = "Game to Delete" };

        _gameRepoMock
            .Setup(r => r.GetByIdAsync(gameId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(gameToDelete);

        _gameRepoMock
            .Setup(r => r.DeleteAsync(gameToDelete, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);


        _unitOfWorkMock.Setup(u => u.Games).Returns(_gameRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.CompleteAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _gameService.DeleteGameById(gameId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        _gameRepoMock.Verify(
            repo => repo.DeleteAsync(
                It.Is<Game>(g => g.Id == gameId),
                It.IsAny<CancellationToken>()
            )
        );
    }

    [Fact]
    public async Task DeleteGameById_WithInvalidId_ShouldFailure()
    {
        // Arrange
        int gameId = 1;

        _gameRepoMock
            .Setup(r => r.GetByIdAsync(gameId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Game?)null);

        _unitOfWorkMock.Setup(u => u.Games).Returns(_gameRepoMock.Object);

        // Act
        var result = await _gameService.DeleteGameById(gameId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Game is not registered");
    }

    [Fact]
    public async Task GetAllGamesWithPromotion_WithGames_ReturnsGamesWithPromotions()
    {
        // Arrange
        var game1 = new Game
        {
            Id = 1,
            Name = "Game1",
            Genre = "Action",
            Price = 30m,
            ReleaseDate = new DateTime(2022, 1, 1),
            Promotions = new List<Promotion>
            {
                new Promotion { Id = 1, Name = "Promo1", DiscountPercentage = 10m }
            }
        };

        var game2 = new Game
        {
            Id = 2,
            Name = "Game2",
            Genre = "Adventure",
            Price = 60m,
            ReleaseDate = new DateTime(2021, 6, 1),
            Promotions = new List<Promotion>
            {
                new Promotion { Id = 2, Name = "Promo2", DiscountPercentage = 20m },
                new Promotion { Id = 3, Name = "Promo3", DiscountPercentage = 5m }
            }
        };

        var gamesWithPromo = new List<Game> { game1, game2 };

        _gamesCustomRepoMock
            .Setup(r => r.GetAllGamesWithPromotion(It.IsAny<CancellationToken>()))
            .ReturnsAsync(gamesWithPromo);

        // Act
        var result = await _gameService.GetAllGamesWithPromotion(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();

        var returnedGames = result.Value.ToList();
        returnedGames.Should().HaveCount(2);

        returnedGames[0].id.Should().Be(game1.Id);
        returnedGames[0].name.Should().Be(game1.Name);
        returnedGames[0].promotions.Should().HaveCount(1);
        returnedGames[0].promotions[0].id.Should().Be(game1.Promotions.First().Id);

        returnedGames[1].id.Should().Be(game2.Id);
        returnedGames[1].promotions.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAllGamesWithPromotion_WhenRepositoryReturnsNull_ShouldReturnFailure()
    {
        // Arrange
        _gamesCustomRepoMock
            .Setup(r => r.GetAllGamesWithPromotion(It.IsAny<CancellationToken>()))
            .ReturnsAsync((ICollection<Game>?)null);

        // Act
        var result = await _gameService.GetAllGamesWithPromotion(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeNull();
        result.Error.Should().Be("No games with promotion");
    }

    [Fact]
    public async Task GetGameById_WithValidId_ShouldReturnGame()
    {
        int gameId = 3;

        var expectedGame = new Game
        {
            Id = gameId,
            Name = "Test Game",
            Genre = "Test Genre",
            ReleaseDate = new DateTime(2023, 1, 1),
            Price = 49.99m
        };

        _gamesCustomRepoMock
            .Setup(r => r.GetGameById(gameId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedGame);

        //Act
        var result = await _gameService.GetGameById(gameId, CancellationToken.None);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.id.Should().Be(gameId);

        _gamesCustomRepoMock.Verify(r => r.GetGameById(gameId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetGameById_WithInvalidId_ShouldReturnFailure()
    {
        int gameId = 999;

        _gamesCustomRepoMock
            .Setup(r => r.GetGameById(gameId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Game?)null);

        //Act
        var result = await _gameService.GetGameById(gameId, CancellationToken.None);

        //Assert
        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeNull();
        result.Error.Should().Be("Game is not registered");
        _gamesCustomRepoMock.Verify(r => r.GetGameById(gameId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateGameById_WithValidId_ShouldUpdateGameSuccessfully()
    {
        // Arrange
        int gameId = 1;

        var existingGame = new Game { Id = gameId, Name = "Old Game", Genre = "Action", Price = 40m, ReleaseDate = new DateTime(2020, 1, 1) };

        var updateGameDto = new UpdateGameDto("Updated Game", "Adventure", new DateTime(2022, 5, 5), 55m);

        _gamesCustomRepoMock
            .Setup(r => r.GetGameByIdUpdate(gameId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingGame);

        _unitOfWorkMock.Setup(u => u.GamesCustom).Returns(_gamesCustomRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.CompleteAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _gameService.UpdateGameById(gameId, updateGameDto, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();

        result.Value!.name.Should().Be(updateGameDto.name);
        result.Value!.genre.Should().Be(updateGameDto.genre);
        result.Value!.price.Should().Be(updateGameDto.price);
        result.Value!.releaseDate.Should().Be(updateGameDto.releaseDate);

        _gamesCustomRepoMock.Verify(r => r.GetGameByIdUpdate(gameId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateGameById_WithInvalidId_ShouldReturnFailure()
    {
        // Arrange
        int gameId = 999;

        var updateGameDto = new UpdateGameDto("Updated Game", "Adventure", new DateTime(2022, 5, 5), 55m);

        _gamesCustomRepoMock
            .Setup(r => r.GetGameByIdUpdate(gameId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Game?)null);

        _unitOfWorkMock.Setup(u => u.GamesCustom).Returns(_gamesCustomRepoMock.Object);

        // Act
        var result = await _gameService.UpdateGameById(gameId, updateGameDto, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeNull();
        result.Error.Should().Be("Game is not registered");
        _gamesCustomRepoMock.Verify(r => r.GetGameByIdUpdate(gameId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateGameById_WithUsedName_ShouldReturnFailure()
    {
        // Arrange
        int gameId = 1;
        var existingGame = new Game { Id = gameId, Name = "Old Game", Genre = "Action", Price = 40m, ReleaseDate = new DateTime(2020, 1, 1) };
        var updateGameDto = new UpdateGameDto("Used Game Name", "Adventure", new DateTime(2022, 5, 5), 55m);

        _gamesCustomRepoMock
            .Setup(r => r.GetGameByIdUpdate(gameId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingGame);

        _gameRepoMock
            .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Game, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _unitOfWorkMock.Setup(u => u.GamesCustom).Returns(_gamesCustomRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Games).Returns(_gameRepoMock.Object);

        // Act
        var result = await _gameService.UpdateGameById(gameId, updateGameDto, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeNull();
        result.Error.Should().Be("A game with this name already exists");

    }
}
