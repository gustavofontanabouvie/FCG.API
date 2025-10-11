using Fcg.Application.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Application.Interfaces;

public interface IUserService
{
    public Task<ResponseUserDto> CreateUser(CreateUserDto createUserDto, CancellationToken cancellationToken);
}
