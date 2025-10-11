using Fcg.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Application.DTOs.Game;

public record ResponseGameDto(string name, string genre, DateTime releaseDate, decimal price);
