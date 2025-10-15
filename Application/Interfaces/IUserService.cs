using Fcg.Application.DTOs.User;
using Fcg.Domain.Common;
using Fcg.Shared;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Application.Interfaces;

public interface IUserService
{
    public Task<Result<ResponseUserDto>> CreateUser(CreateUserDto createUserDto, CancellationToken cancellationToken);
    public Task<Result<ResponseUserDto>> GetUserById(int id, CancellationToken cancellationToken);
}
