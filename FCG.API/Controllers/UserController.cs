using Fcg.Application.DTOs.User;
using Fcg.Application.Interfaces;
using Fcg.Data.Context;
using Fcg.Shared;
using Microsoft.AspNetCore.Authorization;
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

    [SwaggerOperation(summary: "Find a user by ID", Description = "Retrieves the data of a specific user based on their ID")]
    [SwaggerResponse(200, "User found and returned", typeof(ResponseUserDto))]
    [SwaggerResponse(404, "User with that ID was not found")]
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<ResponseUserDto>> GetUserById(int id, CancellationToken cancellationToken)
    {
        var result = await _userService.GetUserById(id, cancellationToken);

        if (!result.IsSuccess)
            return NotFound(new { error = result.Error });

        return Ok(result.Value);
    }

    [SwaggerOperation(summary: "Create a new user", Description = "This endpoint receive the new user data and persists in database")]
    [SwaggerResponse(201, "User created sucessfuly")]
    [SwaggerResponse(409, "Email already in use")]
    [HttpPost]
    public async Task<ActionResult<ResponseUserDto>> PostUser([FromBody] CreateUserDto createUserDto, CancellationToken cancellationToken)
    {
        var result = await _userService.CreateUser(createUserDto, cancellationToken);

        if (!result.IsSuccess)
            return Conflict(new { error = result.Error });

        return CreatedAtAction("GetUserById", new { id = result.Value.id }, result.Value);
    }
}
