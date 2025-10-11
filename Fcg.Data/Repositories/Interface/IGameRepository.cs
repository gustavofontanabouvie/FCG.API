using Fcg.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Data.Repositories.Interface;

public interface IGameRepository
{
    public Task<Game> CreateGame(Game game, CancellationToken cancellationToken);
    bool IsGameRegistered(string name, CancellationToken cancellationToken);
}
