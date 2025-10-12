using Fcg.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Data.Repositories.Interface;
public interface IUnityOfWork
{
    IRepository<User> Users { get; }
    IRepository<Game> Games { get; }
    IRepository<Promotion> Promotions { get; }
    Task<int> SaveChangesAsync();
}
