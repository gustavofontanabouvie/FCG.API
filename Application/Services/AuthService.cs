using Fcg.Application.DTOs.Auth;
using Fcg.Application.DTOs.User;
using Fcg.Application.Interfaces;
using Fcg.Data.Repositories.Interface;
using Fcg.Domain.Common;
using Fcg.Shared;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BCrypt.Net;

namespace Fcg.Application.Services;

public class AuthService : IAuthService
{

    private readonly IUnityOfWork _unityOfWork;
    private readonly IConfiguration _configuration;

    public AuthService(IUnityOfWork unityOfWork, IConfiguration configuration)
    {
        _unityOfWork = unityOfWork;
        _configuration = configuration;
    }

    public async Task<Result<TokenDto>> LoginUser(DTOs.Auth.LoginRequest loginRequest, CancellationToken cancellationToken)
    {
        var users = await _unityOfWork.Users.FindAsync(u => u.Email.Equals(loginRequest.email), cancellationToken);

        var user = users.FirstOrDefault();

        if (user == null)
            return Result<TokenDto>.Failure("Invalid e-mail or password");

        bool verifyPassword = BCrypt.Net.BCrypt.Verify(loginRequest.password, user.Password);

        if (!verifyPassword)
            return Result<TokenDto>.Failure("Invalid e-mail or password");

        var finalToken = GenerateJwtToken(user.Id, user.Email, user.Role.ToString());

        var tokenDto = new TokenDto(finalToken);

        return Result<TokenDto>.Success(tokenDto);

    }

    public string GenerateJwtToken(int userId, string userEmail, string role)
    {
        var key = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
        var issuer = _configuration["Jwt:Issuer"] ?? "FCG.API";
        var audience = _configuration["Jwt:Audience"] ?? "FCG.API";

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email,userEmail),
            new Claim(ClaimTypes.Role,role),
            new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddSeconds(60),
            signingCredentials: credentials
            );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }


}


