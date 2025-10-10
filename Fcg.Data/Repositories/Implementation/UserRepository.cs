using Fcg.Data.Repositories.Interface;
using Fcg.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Data.Repositories.Implementation;

public class UserRepository : IUserRepository
{

    public async Task<User> CreateUser(User user, CancellationToken cancellationToken)
    {
        //_fcgDbContext.Users.Add(user);
        //await _fcgDbContext.SaveChagesAsync();

        return user;
    }

    public bool IsUserRegistered(string email, CancellationToken cancellationToken)
    {
        //    return await _fcgDbContext.Users
        //        .AsNoTracking()
        //        .AnyAsync(user => user.Email.Equals(email));
        return true;
    }
}
