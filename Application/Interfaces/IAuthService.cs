using Fcg.Application.DTOs.Auth;
using Fcg.Application.DTOs.User;
using Fcg.Domain.Common;
using Fcg.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Application.Interfaces;

public interface IAuthService
{
    public Task<Result<TokenDto>> LoginUser(LoginRequest loginRequest, CancellationToken cancellationToken);
    public string GenerateJwtToken(int userId, string email, string role);
}
