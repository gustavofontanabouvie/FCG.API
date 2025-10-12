using Fcg.Application.DTOs.User;
using Fcg.Application.Interfaces;
using Fcg.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Application.Services;

public class AuthService : IAuthService
{
    public Task<UserLoginResponseDto> LoginUser(UserLoginRequest loginRequest, CancellationToken cancellationToken)
    {

    }
}
