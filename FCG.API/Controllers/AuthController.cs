using Fcg.Application.DTOs.Auth;
using Fcg.Application.Interfaces;
using Fcg.Domain.Common;
using Fcg.Shared;
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
    public async Task<ActionResult<TokenDto>> LoginUser([FromBody] LoginRequest loginRequest, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginUser(loginRequest, cancellationToken);

        if (!result.IsSuccess)
            return Unauthorized("Email or password is invalid");

        return Ok(new { Token = result });

    }
}
