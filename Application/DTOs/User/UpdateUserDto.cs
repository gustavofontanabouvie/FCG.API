using Fcg.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Application.DTOs.User;

public record UpdateUserDto(string? name, [EmailAddress] string? email, UserRole? role);
