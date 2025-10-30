using Fcg.Application.DTOs.Promotion;
using Fcg.Application.Interfaces;
using Fcg.Data.Repositories.Interface;
using Fcg.Domain;
using Fcg.Domain.Common;
using Fcg.Shared;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Application.Services;

public class PromotionService : IPromotionService
{
    private readonly IUnityOfWork _unityOfWork;
    private readonly ILogger<PromotionService> _logger;

    public PromotionService(IUnityOfWork unityOfWork, ILogger<PromotionService> logger)
    {
        _logger = logger;
        _unityOfWork = unityOfWork;
    }

    public async Task<Result<PromotionDto>> CreatePromotion(CreatePromotionDto createPromotionDto, CancellationToken cancellationToken)
    {
        bool promotionIsRegistered = await _unityOfWork.Promotions.ExistsAsync(promo => promo.Name.ToLower() == createPromotionDto.name.ToLower(), cancellationToken);
        bool gameRegistered = await _unityOfWork.Games.ExistsAsync(game => game.Id == createPromotionDto.gameId, cancellationToken);

        if (promotionIsRegistered)
        {
            _logger.LogWarning("A promotion with the name {PromotionName} already exists.", createPromotionDto.name);
            return Result<PromotionDto>.Failure("A promotion with this name already exists");
        }

        if (!gameRegistered)
        {
            _logger.LogWarning("No game found with ID {GameId} for promotion creation.", createPromotionDto.gameId);
            return Result<PromotionDto>.Failure("Do not exists a game with that ID");
        }


        var game = await _unityOfWork.Games.GetByIdAsync(createPromotionDto.gameId, cancellationToken);

        Promotion promotion = new()
        {
            Name = createPromotionDto.name,
            DiscountPercentage = createPromotionDto.discountPercentage,
            StartDate = createPromotionDto.startDate,
            EndDate = createPromotionDto.endDate,
            IsActive = createPromotionDto.isActive,
            Game = game
        };

        await _unityOfWork.Promotions.AddAsync(promotion, cancellationToken);

        await _unityOfWork.CompleteAsync(cancellationToken);

        PromotionDto promotionDto = new PromotionDto(promotion.Id, promotion.Name, promotion.DiscountPercentage);

        return Result<PromotionDto>.Success(promotionDto);

    }

    public async Task<Result<PromotionDto>> DeletePromotionById(int id, CancellationToken cancellationToken)
    {
        var promotion = await _unityOfWork.Promotions.GetByIdAsync(id, cancellationToken);

        if (promotion is null)
        {
            _logger.LogWarning("No promotion found with ID {PromotionId} for deletion.", id);
            return Result<PromotionDto>.Failure("Promotion is not registered");
        }

        await _unityOfWork.Promotions.DeleteAsync(promotion, cancellationToken);
        await _unityOfWork.CompleteAsync(cancellationToken);

        var promotionDto = new PromotionDto(promotion.Id, promotion.Name, promotion.DiscountPercentage);

        return Result<PromotionDto>.Success(promotionDto);
    }

    public async Task<Result<PromotionDto>> GetPromotionById(int id, CancellationToken cancellationToken)
    {

        var promotion = await _unityOfWork.Promotions.GetByIdAsync(id, cancellationToken);

        if (promotion is null)
        {
            _logger.LogWarning("No promotion found with ID {PromotionId}.", id);
            return Result<PromotionDto>.Failure("Promotion is not registered");
        }

        var promotionDto = new PromotionDto(promotion.Id, promotion.Name, promotion.DiscountPercentage);

        return Result<PromotionDto>.Success(promotionDto);
    }

    public async Task<Result<PromotionUpdateDto>> UpdatePromotion(int id, PromotionUpdateDto promoUpdateDto, CancellationToken cancellationToken)
    {
        var promotion = await _unityOfWork.Promotions.
             GetByIdAsync(id, cancellationToken);

        if (promotion is null)
        {
            _logger.LogWarning("No promotion found with ID {PromotionId} for update.", id);
            return Result<PromotionUpdateDto>.Failure("Promotion is not registered");
        }

        if (promoUpdateDto.name is not null)
        {
            promotion.Name = promoUpdateDto.name;
        }

        if (promoUpdateDto.percentageDiscount is not null)
        {
            promotion.DiscountPercentage = (decimal)promoUpdateDto.percentageDiscount;
        }

        if (promoUpdateDto.dateStart is not null)
        {
            promotion.StartDate = promoUpdateDto.dateStart.Value;
        }

        if (promoUpdateDto.dateEnd is not null)
        {
            promotion.EndDate = promoUpdateDto.dateEnd.Value;
        }

        if (promoUpdateDto.isActive is not null)
        {
            promotion.IsActive = promoUpdateDto.isActive.Value;
        }

        if (promoUpdateDto.gameId is not null)
        {
            var game = await _unityOfWork.Games.GetByIdAsync(promoUpdateDto.gameId.Value, cancellationToken);

            if (game is null)
            {
                _logger.LogWarning("No game found with ID {GameId} for promotion update.", promoUpdateDto.gameId.Value);
                return Result<PromotionUpdateDto>.Failure("Do not exists a game with that ID");
            }

            promotion.Game = game;
        }

        await _unityOfWork.Promotions.UpdateAsync(promotion, cancellationToken);

        await _unityOfWork.CompleteAsync(cancellationToken);

        var responseUpdateDto = new PromotionUpdateDto(promotion.Name, promotion.DiscountPercentage, promotion.StartDate, promotion.EndDate, promotion.IsActive, promotion.Game?.Id);

        return Result<PromotionUpdateDto>.Success(responseUpdateDto);

    }

}
