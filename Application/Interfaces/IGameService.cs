using Fcg.Application.DTOs.Game;
using Fcg.Domain.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Application.Interfaces;

public interface IGameService
{
    public Task<Result<ResponseGameDto>> CreateGame(CreateGameDto createGameDto, CancellationToken cancellationToken);
    public Task<Result<IEnumerable<ResponseGameDto>>> GetAllGamesWithPromotion(CancellationToken cancellationToken);
    public Task<Result<ResponseGameDto>> GetGameById(int id, CancellationToken cancellationToken);
}
