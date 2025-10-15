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

    public async Task<Result<ResponseUserDto>> CreateUser(CreateUserDto createUserDto, CancellationToken cancellationToken)
    {
        bool isUserRegistered = await _unityOfWork.Users.ExistsAsync(u => u.Email == createUserDto.email, cancellationToken);

        if (isUserRegistered)
        {
            return Result<ResponseUserDto>.Failure("E-mail already in use");
        }

        var user = new User
        {
            Email = createUserDto.email,
            Name = createUserDto.name,
            Password = createUserDto.password,
            Role = UserRole.User
        };

        await _unityOfWork.Users.AddAsync(user, cancellationToken);

        await _unityOfWork.SaveChangesAsync();

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
}
