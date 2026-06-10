using System.Security.Claims;
using Ledger.Application.Commands.Participante;
using Ledger.Application.Queries.Convite;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ledger.Api.Controllers;

[ApiController]
[Route("api/convites")]
[Authorize]
public class ConvitesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ConvitesController(IMediator mediator) => _mediator = mediator;

    private Guid UsuarioId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? throw new UnauthorizedAccessException());

    /// <summary>Lista os convites pendentes do usuário autenticado.</summary>
    [HttpGet("pendentes")]
    public async Task<IActionResult> Pendentes(CancellationToken ct)
    {
        var convites = await _mediator.Send(new ListarConvitesPendentesQuery(UsuarioId), ct);
        return Ok(convites);
    }

    /// <summary>Aceita um convite pelo token.</summary>
    [HttpPost("{token}/aceitar")]
    public async Task<IActionResult> Aceitar(string token, CancellationToken ct)
    {
        var participante = await _mediator.Send(new AceitarConviteCommand(token, UsuarioId), ct);
        return Ok(participante);
    }

    /// <summary>Recusa um convite pelo token.</summary>
    [HttpPost("{token}/recusar")]
    public async Task<IActionResult> Recusar(string token, CancellationToken ct)
    {
        var convite = await _mediator.Send(new RecusarConviteCommand(token, UsuarioId), ct);
        return Ok(convite);
    }
}
