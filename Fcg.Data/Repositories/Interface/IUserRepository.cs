using Fcg.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Data.Repositories.Interface;

public interface IUserRepository
{
    public Task<User> CreateUser(User user, CancellationToken cancellationToken);
    bool IsUserRegistered(string email, CancellationToken cancellationToken);
}
