using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Application.DTOs.User;

public record ResponseUserDto(int id, string name, string email);

public record UserLoginRequest(string email, string senha);

