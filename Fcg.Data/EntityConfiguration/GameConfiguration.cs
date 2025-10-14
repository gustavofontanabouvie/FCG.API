using Fcg.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Data.EntityConfiguration;

public class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.Property(g => g.Genre).HasColumnType("varchar(200)");
        builder.Property(g => g.Name).HasColumnType("varchar(200)");
        builder.Property(g => g.Price).HasColumnType("decimal(10,2)");

        builder.HasData(new Game
        {
            Id = 1,
            Name = "Jogo teste",
            Genre = "Aventura",
            Price = 99.99m,
            ReleaseDate = new DateTime(2025, 10, 10).ToUniversalTime()
        });
    }
}
