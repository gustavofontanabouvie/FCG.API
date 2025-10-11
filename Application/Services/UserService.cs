using Fcg.Application.DTOs.User;
using Fcg.Application.Interfaces;
using Fcg.Data.Repositories.Interface;
using Fcg.Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ResponseUserDto> CreateUser(CreateUserDto createUserDto, CancellationToken cancellationToken)
    {
        var user = new User
        {
            Email = createUserDto.email,
            Name = createUserDto.name,
            Password = createUserDto.password,
            Role = (UserRole)1
        };

        bool isUserRegistered = _userRepository.IsUserRegistered(user.Email, cancellationToken);

        if (!isUserRegistered)
        {
            await _userRepository.CreateUser(user, cancellationToken);

            var responseDto = new ResponseUserDto(user.Name, user.Email);

            return responseDto;
        }

        return null;
    }
}
