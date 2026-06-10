using System.Security.Claims;
using Ledger.Application.Commands.Participante;
using Ledger.Application.DTOs.Participante;
using Ledger.Application.Queries.Participante;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ledger.Api.Controllers;

[ApiController]
[Route("api/cofres/{cofreId:guid}/participantes")]
[Authorize]
public class ParticipantesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ParticipantesController(IMediator mediator) => _mediator = mediator;

    private Guid UsuarioId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? throw new UnauthorizedAccessException());

    [HttpGet]
    public async Task<IActionResult> Listar(Guid cofreId, CancellationToken ct)
    {
        var participantes = await _mediator.Send(new ListarParticipantesQuery(cofreId), ct);
        return Ok(participantes);
    }

    /// <summary>Envia convite por e-mail. O participante só entra após aceitar.</summary>
    [HttpPost]
    public async Task<IActionResult> Convidar(Guid cofreId, [FromBody] CriarParticipanteRequest request, CancellationToken ct)
    {
        var convite = await _mediator.Send(
            new EnviarConviteCommand(cofreId, UsuarioId, request.UsuarioId), ct);
        return Created(string.Empty, convite);
    }

    [HttpDelete("{participanteId:guid}")]
    public async Task<IActionResult> Remover(Guid cofreId, Guid participanteId, CancellationToken ct)
    {
        var removido = await _mediator.Send(
            new RemoverParticipanteCommand(cofreId, participanteId), ct);
        return removido ? NoContent() : NotFound();
    }
}
