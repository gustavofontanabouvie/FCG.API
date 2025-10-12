using Fcg.Application.DTOs.User;
using Fcg.Application.Interfaces;
using Fcg.Data.Context;
using Fcg.Shared;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FCG.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [SwaggerOperation(summary: "Create a new user", Description = "This endpoint receive the new user data and persists in database")]
    [SwaggerResponse(201, "User created sucessfuly")]
    [SwaggerResponse(422, "Email already in use")]
    [HttpPost]
    public async Task<ActionResult<ResponseUserDto>> PostUser([FromBody] CreateUserDto createUserDto, CancellationToken cancellationToken)
    {
        var response = await _userService.CreateUser(createUserDto, cancellationToken);

        if (response is null)
            return UnprocessableEntity();

        return CreatedAtAction("GetUser", new { name = response.name }, response);
    }
}
