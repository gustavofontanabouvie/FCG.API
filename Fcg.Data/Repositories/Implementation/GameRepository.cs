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

    public bool IsGameRegistered(string name, CancellationToken cancellationToken)
    {
        //return await fcgDbContext.Games.AsNoTracking()
        //        .AnyAsync(game => game.Name.Equals(name));
        return true;
    }
}
