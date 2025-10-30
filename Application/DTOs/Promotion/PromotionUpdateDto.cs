using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Application.DTOs.Promotion;

public record PromotionUpdateDto(string? name, decimal? percentageDiscount, DateTime? dateStart, DateTime? dateEnd, bool? isActive, int? gameId);

