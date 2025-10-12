using Fcg.Application.DTOs.User;
using Fcg.Application.Interfaces;
using Fcg.Shared;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace FCG.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<UserLoginResponseDto> LoginUser([FromBody] UserLoginRequest loginRequest, CancellationToken cancellationToken)
    {
        var loginResponse = await _authService.LoginUser(loginRequest, cancellationToken);

        return loginResponse;
    }
}
