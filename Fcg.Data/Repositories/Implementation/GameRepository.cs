﻿using Fcg.Data.Context;
using Fcg.Data.Repositories.Interface;
using Fcg.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Data.Repositories.Implementation;

public class GameRepository : IGameRepository
{
    private readonly FcgDbContext _dbContext;

    public GameRepository(FcgDbContext dbContext)
    {
        this._dbContext = dbContext;
    }

    public async Task<Game> CreateGame(Game game, CancellationToken cancellationToken)
    {
        await _dbContext.Games.AddAsync(game);
        await _dbContext.SaveChangesAsync();

        return game;
    }

    public async Task<IEnumerable<Game>> GetAllGamesWithPromotion(CancellationToken cancellationToken)
    {
        return await _dbContext.Games
            .AsNoTracking()
            .Include(g => g.Promotions)
            .Where(g => g.Promotions.Any())
            .ToListAsync(cancellationToken);
    }

    public async Task<Game?> GetGameById(int id, CancellationToken cancellationToken)
    {
        return await _dbContext.Games
            .AsNoTracking()
            .Include(g => g.Promotions)
            .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
    }

    public async Task<Game?> GetGameByIdUpdate(int id, CancellationToken cancellationToken)
    {
        return await _dbContext.Games
            .Include(g => g.Promotions)
            .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
    }
}
