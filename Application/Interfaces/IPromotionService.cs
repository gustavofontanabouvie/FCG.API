using Fcg.Application.DTOs.Promotion;
using Fcg.Domain.Common;
using Fcg.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Application.Interfaces;

public interface IPromotionService
{
    public Task<Result<PromotionDto>> CreatePromotion(CreatePromotionDto createPromotionDto, CancellationToken cancellationToken);
    public Task<Result<PromotionDto>> GetPromotionById(int id, CancellationToken cancellationToken);
}
