using System;
using System.ComponentModel.DataAnnotations;

namespace Fcg.Application.DTOs.User;

public record CreateUserDto(
    [Required, MinLength(3)] string name,
    [Required, EmailAddress] string email,
    [Required, MinLength(8), RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[^A-Za-z\d]).{8,}$",
        ErrorMessage = "Password must have letters, numbers and special characters.")] string password);
