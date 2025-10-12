using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Shared;

public record UserLoginResponseDto(string email, string token);
