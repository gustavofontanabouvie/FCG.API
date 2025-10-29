using Fcg.Application.DTOs.Game;
using Fcg.Application.DTOs.User;
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
}

