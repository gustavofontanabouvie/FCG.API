using BCrypt.Net;
using Fcg.Application.DTOs.User;
using Fcg.Application.Interfaces;
using Fcg.Data.Repositories.Interface;
using Fcg.Domain;
using Fcg.Domain.Common;
using Fcg.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Application.Services;

public class UserService : IUserService
{
    private readonly IUnityOfWork _unityOfWork;

    public UserService(IUnityOfWork unityOfWork)
    {
        _unityOfWork = unityOfWork;
    }

    public async Task<Result<ResponseUserDto>> CreateAdminUser(CreateUserDto createUserDto, CancellationToken cancellationToken)
    {
        bool isUserRegistered = await _unityOfWork.Users.ExistsAsync(u => u.Email == createUserDto.email, cancellationToken);

        if (isUserRegistered)
        {
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
            return Result<ResponseUserDto>.Failure("User is not registered");

        ResponseUserDto response = new ResponseUserDto(user.Id, user.Name, user.Email);

        return Result<ResponseUserDto>.Success(response);
    }

    public async Task<Result<ResponseUserDto>> UpdateUser(int id, UpdateUserDto updateDto, CancellationToken cancellationToken)
    {
        var user = await _unityOfWork.Users.GetByIdAsync(id, cancellationToken);

        if (user is null)
            return Result<ResponseUserDto>.Failure("User is not registered");

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
