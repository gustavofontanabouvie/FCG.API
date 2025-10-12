using Fcg.Data.Context;
using Fcg.Data.Repositories.Interface;
using Fcg.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Data.Repositories.Implementation;

public class GameRepository : IGameRepository
{
    public async Task<Game> CreateGame(Game game, CancellationToken cancellationToken)
    {
        //await fcgDbContext.Games.AddAsync(game);
        return game;
    }

    public async Task<IEnumerable<Game>> GetAllGamesWithPromotion(CancellationToken cancellationToken)
    {
        //    return await fcgdbcontext.games
        //        .AsNoTracking()
        //        .Include(game => game.promotion)
        //        .Select(game => new Game
        //        {
        //            id = game.id,
        //            genre = game.genre,
        //            name = game.name,
        //            price = game.price,
        //            releasedate = game.releasedate,
        //            promotion = game.promotion
        //        })
        //        .Where(game => game.Promotion.IsActive == true)
        //        .tolistAsync();
        return null;
    }

    public bool IsGameRegistered(string name, CancellationToken cancellationToken)
    {
        //return await fcgDbContext.Games.AsNoTracking()
        //        .AnyAsync(game => game.Name.Equals(name));
        return true;
    }
}
