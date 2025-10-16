using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Application.DTOs.Promotion;

public record CreatePromotionDto(string name, int gameId, decimal discountPercentage, DateTime startDate, DateTime endDate, bool isActive);
