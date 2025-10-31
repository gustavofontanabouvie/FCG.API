using Fcg.Application.DTOs.Promotion;
using Fcg.Application.Services;
using Fcg.Data.Repositories.Interface;
using Fcg.Domain;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Tests;

public class PromotionTest
{

    private readonly Mock<IUnityOfWork> _unitOfWorkMock;
    private readonly PromotionService _promotionService;


    public PromotionTest()
    {
        _unitOfWorkMock = new Mock<IUnityOfWork>();
        _promotionService = new PromotionService(_unitOfWorkMock.Object, new Mock<ILogger<PromotionService>>().Object);
    }

    [Fact]
    public async Task CreatePromotion_WithUnusedName_ShouldCreateSuccessfully()
    {
        //Arrange
        CreatePromotionDto promo = new CreatePromotionDto("new game", 1, 10, new DateTime(2025, 12, 7), new DateTime(2026, 1, 15), true);

        var game = new Game
        {
            Id = 1,
            Name = "Existing Game",
            Genre = "Action",
            ReleaseDate = new DateTime(2023, 5, 20),
            Price = 59.99M
        };

        var promoRepoMock = new Mock<IRepository<Promotion>>();
        var gameRepoMock = new Mock<IRepository<Game>>();

        promoRepoMock
            .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Promotion, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        gameRepoMock
            .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Game, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        gameRepoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(game);

        promoRepoMock
            .Setup(r => r.AddAsync(It.IsAny<Promotion>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Promotion p, CancellationToken ct) => p);

        _unitOfWorkMock.Setup(u => u.Promotions).Returns(promoRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Games).Returns(gameRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.CompleteAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        //Act
        var result = await _promotionService.CreatePromotion(promo, CancellationToken.None);

        //Assert

        result.IsSuccess.Should().BeTrue();
        result.Value.name.Should().Be(promo.name);
        result.Error.Should().BeNull();

        promoRepoMock.Verify(r => r.AddAsync(It.Is<Promotion>(p => p.Name == promo.name), It.IsAny<CancellationToken>()), Times.Once);

    }

    [Fact]
    public async Task CreatePromotion_WithExistingName_ShouldFail()
    {
        //Arrange
        CreatePromotionDto promo = new("existing promo", 1, 10, new DateTime(2025, 12, 7), new DateTime(2026, 1, 15), true);

        var promoRepoMock = new Mock<IRepository<Promotion>>();
        promoRepoMock
            .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Promotion, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var gameRepoMock = new Mock<IRepository<Game>>();

        gameRepoMock
            .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Game, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        gameRepoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Game { Id = 1, Name = "Any", Genre = "Any", ReleaseDate = DateTime.UtcNow, Price = 1 });

        _unitOfWorkMock.Setup(u => u.Promotions).Returns(promoRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Games).Returns(gameRepoMock.Object);

        //Act
        var result = await _promotionService.CreatePromotion(promo, CancellationToken.None);

        //Assert
        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeNull();
        result.Error.Should().Be("A promotion with this name already exists");
        promoRepoMock.Verify(r => r.AddAsync(It.IsAny<Promotion>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreatePromotion_WithNonExistentGameId_ShouldFail()
    {
        //Arrange
        CreatePromotionDto promo = new("new promo", 99, 10, new DateTime(2025, 12, 7), new DateTime(2026, 1, 15), true);

        var promoRepoMock = new Mock<IRepository<Promotion>>();

        promoRepoMock
            .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Promotion, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var gameRepoMock = new Mock<IRepository<Game>>();

        gameRepoMock
            .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Game, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _unitOfWorkMock.Setup(u => u.Promotions).Returns(promoRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Games).Returns(gameRepoMock.Object);

        //Act
        var result = await _promotionService.CreatePromotion(promo, CancellationToken.None);

        //Assert
        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeNull();
        result.Error.Should().Be("Do not exists a game with that ID");
        promoRepoMock.Verify(r => r.AddAsync(It.IsAny<Promotion>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeletePromotionById_WithValidId_ShouldDeleteSuccessfully()
    {
        //Arrange
        int promoId = 1;

        var promo = new Promotion
        {
            Id = promoId,
            Name = "Promo to Delete",
            DiscountPercentage = 15,
            StartDate = new DateTime(2025, 11, 1),
            EndDate = new DateTime(2025, 12, 1),
            IsActive = true
        };

        var promoRepoMock = new Mock<IRepository<Promotion>>();

        promoRepoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(promo);

        promoRepoMock
            .Setup(r => r.DeleteAsync(It.IsAny<Promotion>(), It.IsAny<CancellationToken>()));

        _unitOfWorkMock.Setup(u => u.Promotions).Returns(promoRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.CompleteAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        //Act
        var result = await _promotionService.DeletePromotionById(promoId, CancellationToken.None);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.name.Should().Be(promo.Name);
        result.Error.Should().BeNull();
        promoRepoMock.Verify(r => r.DeleteAsync(It.Is<Promotion>(p => p.Id == promoId), CancellationToken.None));
    }

    [Fact]
    public async Task DeletePromotionById_WithInvalidId_ShouldFail()
    {
        //Arrange
        int promoId = 99;

        var promoRepoMock = new Mock<IRepository<Promotion>>();

        promoRepoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Promotion?)null);

        _unitOfWorkMock.Setup(u => u.Promotions).Returns(promoRepoMock.Object);

        //Act
        var result = await _promotionService.DeletePromotionById(promoId, CancellationToken.None);

        //Assert
        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeNull();
        result.Error.Should().Be("Promotion is not registered");
        promoRepoMock.Verify(r => r.DeleteAsync(It.IsAny<Promotion>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetPromotionById_WithValidId_ShouldReturnPromotion()
    {
        //Arrange
        int promoId = 1;

        var promo = new Promotion
        {
            Id = promoId,
            Name = "Existing Promo",
            DiscountPercentage = 20,
            StartDate = new DateTime(2025, 10, 1),
            EndDate = new DateTime(2025, 11, 1),
            IsActive = true
        };

        var promoRepoMock = new Mock<IRepository<Promotion>>();

        promoRepoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(promo);

        _unitOfWorkMock.Setup(u => u.Promotions).Returns(promoRepoMock.Object);

        //Act
        var result = await _promotionService.GetPromotionById(promoId, CancellationToken.None);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.name.Should().Be(promo.Name);
        result.Error.Should().BeNull();
    }

    [Fact]
    public async Task GetPromotionById_WithInvalidId_ShouldFail()
    {
        //Arrange
        int promoId = 99;

        var promoRepoMock = new Mock<IRepository<Promotion>>();

        promoRepoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Promotion?)null);

        _unitOfWorkMock.Setup(u => u.Promotions).Returns(promoRepoMock.Object);

        //Act
        var result = await _promotionService.GetPromotionById(promoId, CancellationToken.None);

        //Assert
        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeNull();
        result.Error.Should().Be("Promotion is not registered");
    }

    [Fact]
    public async Task UpdatePromotionById_WithUnusedName_ShouldUpdateSuccesfully()
    {
        int promoId = 1;

        var updateDto = new PromotionUpdateDto("Updated Promo", 5, new DateTime(2025, 9, 1), new DateTime(2025, 10, 1), true, 1);

        var existingPromo = new Promotion
        {
            Id = promoId,
            Name = "Old Promo",
            DiscountPercentage = 10,
            StartDate = new DateTime(2025, 8, 1),
            EndDate = new DateTime(2025, 9, 1),
            IsActive = false
        };

        var game = new Game
        {
            Id = 1,
            Name = "Existing Game",
            Genre = "Adventure",
            ReleaseDate = new DateTime(2023, 6, 15),
            Price = 49.99M
        };

        var repoPromoMock = new Mock<IRepository<Promotion>>();
        var repoGameMock = new Mock<IRepository<Game>>();

        repoPromoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingPromo);

        repoPromoMock
            .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Promotion, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        repoGameMock
            .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Game, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        repoGameMock
           .Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync(game);

        repoPromoMock
           .Setup(r => r.UpdateAsync(It.IsAny<Promotion>(), It.IsAny<CancellationToken>()))
           .Returns(Task.CompletedTask);

        _unitOfWorkMock.Setup(u => u.Promotions).Returns(repoPromoMock.Object);
        _unitOfWorkMock.Setup(u => u.Games).Returns(repoGameMock.Object);
        _unitOfWorkMock.Setup(u => u.CompleteAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        //Act
        var result = await _promotionService.UpdatePromotion(promoId, updateDto, CancellationToken.None);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.name.Should().Be(updateDto.name);
        result.Error.Should().BeNull();

        repoPromoMock.Verify(r => r.UpdateAsync(It.Is<Promotion>(p => p.Id == promoId && p.Name == updateDto.name), It.IsAny<CancellationToken>()), Times.Once);

    }

    [Fact]
    public async Task UpdatePromotionById_WithUsedName_ShouldFail()
    {
        int promoId = 1;

        var updateDto = new PromotionUpdateDto("Used Promo Name", 5, new DateTime(2025, 9, 1), new DateTime(2025, 10, 1), true, 1);

        var existingPromo = new Promotion
        {
            Id = promoId,
            Name = "Old Promo",
            DiscountPercentage = 10,
            StartDate = new DateTime(2025, 8, 1),
            EndDate = new DateTime(2025, 9, 1),
            IsActive = false
        };

        var game = new Game
        {
            Id = 1,
            Name = "Existing Game",
            Genre = "Adventure",
            ReleaseDate = new DateTime(2023, 6, 15),
            Price = 49.99M
        };

        var repoPromoMock = new Mock<IRepository<Promotion>>();
        var repoGameMock = new Mock<IRepository<Game>>();

        repoPromoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingPromo);

        repoPromoMock
            .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Promotion, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        repoGameMock
            .Setup
            (r => r.ExistsAsync(It.IsAny<Expression<Func<Game, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _unitOfWorkMock.Setup(u => u.Promotions).Returns(repoPromoMock.Object);
        _unitOfWorkMock.Setup(u => u.Games).Returns(repoGameMock.Object);

        //Act
        var result = await _promotionService.UpdatePromotion(promoId, updateDto, CancellationToken.None);

        //Assert
        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeNull();
        result.Error.Should().Be("A promotion with this name already exists");
        repoPromoMock.Verify(r => r.UpdateAsync(It.IsAny<Promotion>(), It.IsAny<CancellationToken>()), Times.Never);

    }

    [Fact]
    public async Task UpdatePromotionById_WithInvalidGameId_ShouldFail()
    {
        int promoId = 1;

        var updateDto = new PromotionUpdateDto("Updated Promo", 5, new DateTime(2025, 9, 1), new DateTime(2025, 10, 1), true, 99);

        var existingPromo = new Promotion
        {
            Id = promoId,
            Name = "Old Promo",
            DiscountPercentage = 10,
            StartDate = new DateTime(2025, 8, 1),
            EndDate = new DateTime(2025, 9, 1),
            IsActive = false
        };

        var repoPromoMock = new Mock<IRepository<Promotion>>();
        var repoGameMock = new Mock<IRepository<Game>>();

        repoPromoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingPromo);

        repoPromoMock
            .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Promotion, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        repoGameMock
            .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Game, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _unitOfWorkMock.Setup(u => u.Promotions).Returns(repoPromoMock.Object);
        _unitOfWorkMock.Setup(u => u.Games).Returns(repoGameMock.Object);

        //Act
        var result = await _promotionService.UpdatePromotion(promoId, updateDto, CancellationToken.None);

        //Assert
        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeNull();
        result.Error.Should().Be("Do not exists a game with that ID");
        repoPromoMock.Verify(r => r.UpdateAsync(It.IsAny<Promotion>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}