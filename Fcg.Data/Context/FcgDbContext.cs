using Fcg.Data.EntityConfiguration;
using Fcg.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Data.Context;

public class FcgDbContext : DbContext
{
    public FcgDbContext(DbContextOptions<FcgDbContext> dbContextOptions) : base(dbContextOptions)
    {
    }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    optionsBuilder.UseNpgsql("Host=localhost; Database=FcgDataBase; Username=postgres; Password=postgres");
    //    base.OnConfiguring(optionsBuilder);
    //}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserConfiguration).Assembly);


        modelBuilder.Entity<PurchasedGame>()
            .HasOne<User>()
            .WithMany(u => u.PurchasedGames)
            .HasForeignKey(pg => pg.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<User>()
            .HasMany(u => u.PurchasedGames)
            .WithOne(pg => pg.User)
            .HasForeignKey(pg => pg.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<PurchasedGame>()
            .HasOne(pg => pg.Game)
            .WithMany()
            .HasForeignKey(pg => pg.GameId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Promotion>()
            .HasOne<Game>()
            .WithMany(g => g.Promotions)
            .HasForeignKey(p => p.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Game>()
            .HasMany(g => g.Promotions)
            .WithOne(p => p.Game)
            .HasForeignKey(p => p.GameId)
            .OnDelete(DeleteBehavior.NoAction);

    }
    public DbSet<User> Users { get; set; }

    public DbSet<Game> Games { get; set; }

    public DbSet<Promotion> Promotions { get; set; }

    public DbSet<PurchasedGame> Purchases { get; set; }

}
