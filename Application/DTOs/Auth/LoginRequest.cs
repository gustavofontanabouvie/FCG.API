using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Application.DTOs.Auth;

public record LoginRequest(
    [Required(ErrorMessage ="E-mail is required")]
    [EmailAddress(ErrorMessage ="E-mail format is invalid")]
    string email,
    [Required][RegularExpression(
    @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
    ErrorMessage ="The password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one number, and one special symbol (@$!%*?&).")]
    string password
    );

