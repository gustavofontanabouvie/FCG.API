using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Application.DTOs.User;

public record CreateUserDto
    ([Required] string name,
    [Required][EmailAddress] string email,
    [Required][RegularExpression(
    @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
    ErrorMessage ="The password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one number, and one special symbol (@$!%*?&).")]
    string password);
