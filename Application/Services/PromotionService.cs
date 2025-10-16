using Fcg.Application.DTOs.Promotion;
using Fcg.Application.Interfaces;
using Fcg.Data.Repositories.Interface;
using Fcg.Domain;
using Fcg.Domain.Common;
using Fcg.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Application.Services;

public class PromotionService : IPromotionService
{
    private readonly IUnityOfWork _unityOfWork;

    public PromotionService(IUnityOfWork unityOfWork)
    {
        _unityOfWork = unityOfWork;
    }

    public async Task<Result<PromotionDto>> CreatePromotion(CreatePromotionDto createPromotionDto, CancellationToken cancellationToken)
    {
        bool promotionIsRegistered = await _unityOfWork.Promotions.ExistsAsync(promo => promo.Name.ToLower() == createPromotionDto.name.ToLower(), cancellationToken);
        bool gameRegistered = await _unityOfWork.Games.ExistsAsync(game => game.Id == createPromotionDto.gameId, cancellationToken);

        if (promotionIsRegistered)
            return Result<PromotionDto>.Failure("A promotion with this name already exists");

        if (!gameRegistered)
            return Result<PromotionDto>.Failure("Do not exists a game with that ID");

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

    public async Task<Result<PromotionDto>> GetPromotionById(int id, CancellationToken cancellationToken)
    {

        var promotion = await _unityOfWork.Promotions.GetByIdAsync(id, cancellationToken);

        if (promotion is null)
            return Result<PromotionDto>.Failure("Promotion is not registered");

        var promotionDto = new PromotionDto(promotion.Id, promotion.Name, promotion.DiscountPercentage);

        return Result<PromotionDto>.Success(promotionDto);
    }
}
