using Fcg.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Application.DTOs.Game;

public record CreateGameDto(
    [Required(ErrorMessage = "O nome é obrigatório")]
    string name,
    [Required(ErrorMessage = "O gênero é obrigatório")]
    string genre,
    DateTime releaseDate,
    [Required(ErrorMessage= "O preço do jogo é obrigatório")]
    [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "O preço deve ser maior que zero")]
    decimal price);
