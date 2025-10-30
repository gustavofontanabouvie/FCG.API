using BCrypt.Net;
using Fcg.Application.DTOs.User;
using Fcg.Application.Interfaces;
using Fcg.Data.Repositories.Interface;
using Fcg.Domain;
using Fcg.Domain.Common;
using Fcg.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Application.Services;

public class UserService : IUserService
{
    private readonly IUnityOfWork _unityOfWork;
    private readonly ILogger<UserService> _logger;

    public UserService(IUnityOfWork unityOfWork, ILogger<UserService> logger)
    {
        _logger = logger;
        _unityOfWork = unityOfWork;
    }

    public async Task<Result<ResponseUserDto>> CreateAdminUser(CreateUserDto createUserDto, CancellationToken cancellationToken)
    {
        bool isUserRegistered = await _unityOfWork.Users.ExistsAsync(u => u.Email == createUserDto.email, cancellationToken);

        if (isUserRegistered)
        {
            _logger.LogWarning("Attempt to create an admin user with an already registered email: {Email}", createUserDto.email);
            return Result<ResponseUserDto>.Failure("E-mail already in use");
        }

        string hashPassword = BCrypt.Net.BCrypt.HashPassword(createUserDto.password, 12);

        var user = new User
        {
            Email = createUserDto.email,
            Name = createUserDto.name,
            Password = hashPassword,
            Role = UserRole.Admin
        };

        await _unityOfWork.Users.AddAsync(user, cancellationToken);

        await _unityOfWork.CompleteAsync(cancellationToken);

        var responseDto = new ResponseUserDto(user.Id, user.Name, user.Email);

        return Result<ResponseUserDto>.Success(responseDto);
    }

    public async Task<Result<ResponseUserDto>> CreateUser(CreateUserDto createUserDto, CancellationToken cancellationToken)
    {
        bool isUserRegistered = await _unityOfWork.Users.ExistsAsync(u => u.Email == createUserDto.email, cancellationToken);

        if (isUserRegistered)
        {
            _logger.LogWarning("E-mail already in use");
            return Result<ResponseUserDto>.Failure("E-mail already in use");
        }

        string hashPassword = BCrypt.Net.BCrypt.HashPassword(createUserDto.password, 12);

        var user = new User
        {
            Email = createUserDto.email,
            Name = createUserDto.name,
            Password = hashPassword,
            Role = UserRole.User
        };

        await _unityOfWork.Users.AddAsync(user, cancellationToken);

        await _unityOfWork.CompleteAsync(cancellationToken);

        var responseDto = new ResponseUserDto(user.Id, user.Name, user.Email);

        return Result<ResponseUserDto>.Success(responseDto);

    }

    public async Task<Result<ResponseUserDto>> GetUserById(int id, CancellationToken cancellationToken)
    {
        var user = await _unityOfWork.Users.GetByIdAsync(id, cancellationToken);

        if (user is null)
        {
            _logger.LogWarning("User is not registered");
            return Result<ResponseUserDto>.Failure("User is not registered");
        }

        ResponseUserDto response = new ResponseUserDto(user.Id, user.Name, user.Email);

        return Result<ResponseUserDto>.Success(response);
    }

    public async Task<Result<ResponseUserDto>> UpdateUser(int id, UpdateUserDto updateDto, CancellationToken cancellationToken)
    {
        var user = await _unityOfWork.Users.GetByIdAsync(id, cancellationToken);

        if (user is null)
        {
            _logger.LogWarning("User is not registered");
            return Result<ResponseUserDto>.Failure("User is not registered");
        }

        if (updateDto.name is not null)
        {
            user.Name = updateDto.name;
        }

        if (updateDto.email is not null)
        {
            user.Email = updateDto.email;
        }

        if (updateDto.role is not null)
        {
            if (!Enum.IsDefined(typeof(UserRole), updateDto.role))
            {
                _logger.LogWarning("Invalid role provided: {Role}", updateDto.role);
                return Result<ResponseUserDto>.Failure("Invalid role");
            }

            user.Role = (UserRole)updateDto.role;
        }

        await _unityOfWork.Users.UpdateAsync(user, cancellationToken);
        await _unityOfWork.CompleteAsync(cancellationToken);

        ResponseUserDto response = new ResponseUserDto(user.Id, user.Name, user.Email);

        return Result<ResponseUserDto>.Success(response);
    }
}
