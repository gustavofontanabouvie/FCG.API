using Fcg.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Data.EntityConfiguration;

public class PromotionConfiguration : IEntityTypeConfiguration<Promotion>
{
    public void Configure(EntityTypeBuilder<Promotion> builder)
    {
        builder.Property(p => p.Name).HasColumnType("varchar(200)");

        builder.HasData(
                new Promotion
                {
                    Id = 1,
                    GameId = 1,
                    IsActive = true,
                    Name = "Promoção dia das crianças",
                    StartDate = new DateTime(2025, 10, 10).ToUniversalTime(),
                    EndDate = new DateTime(2025, 10, 13).ToUniversalTime(),
                    DiscountPercentage = 10
                });
    }
}
