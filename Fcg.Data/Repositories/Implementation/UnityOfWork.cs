using Fcg.Data.Context;
using Fcg.Data.Repositories.Interface;
using Fcg.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Data.Repositories.Implementation
{
    public class UnityOfWork : IUnityOfWork
    {
        private readonly FcgDbContext _dbContext;

        private IRepository<User>? _users;
        private IRepository<Game>? _games;
        private IRepository<Promotion>? _promotions;
        private IGameRepository _gamesCustom;

        public UnityOfWork(FcgDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IRepository<User> Users => _users ??= new Repository<User>(_dbContext);
        public IRepository<Game> Games => _games ??= new Repository<Game>(_dbContext);
        public IRepository<Promotion> Promotions => _promotions ??= new Repository<Promotion>(_dbContext);
        public IGameRepository GamesCustom => _gamesCustom ??= new GameRepository(_dbContext);

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public async Task<int> CompleteAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
