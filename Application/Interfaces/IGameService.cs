﻿using Fcg.Application.DTOs.Game;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Application.Interfaces;

public interface IGameService
{
    public Task<ResponseGameDto> CreateGame(CreateGameDto createGameDto, CancellationToken cancellationToken);
    public Task<ActionResult<IEnumerable<ResponseGameDto>>> GetAllGamesWithPromotion(CancellationToken cancellationToken);
}
