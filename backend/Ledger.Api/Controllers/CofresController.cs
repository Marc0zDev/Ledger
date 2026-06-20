using System.Security.Claims;
using Ledger.Application.Commands.Cofre;
using Ledger.Application.Commands.Participante;
using Ledger.Application.DTOs.Cofre;
using Ledger.Application.DTOs.Participante;
using Ledger.Application.Queries.Cofre;
using Ledger.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ledger.Api.Controllers;

[ApiController]
[Route("api/cofres")]
[Authorize]
public class CofresController : ControllerBase
{
    private readonly IMediator _mediator;

    public CofresController(IMediator mediator) => _mediator = mediator;

    private Guid UsuarioId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? throw new UnauthorizedAccessException());

    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken ct)
    {
        var cofres = await _mediator.Send(new ListarCofresQuery(UsuarioId), ct);
        return Ok(cofres);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId(Guid id, CancellationToken ct)
    {
        var cofre = await _mediator.Send(new ObterCofreQuery(id), ct);
        return cofre is null ? NotFound() : Ok(cofre);
    }

    [HttpGet("{id:guid}/detalhes")]
    public async Task<IActionResult> ObterComDetalhes(Guid id, CancellationToken ct)
    {
        var cofre = await _mediator.Send(new ObterCofreComDetalhesQuery(id), ct);
        return cofre is null ? NotFound() : Ok(cofre);
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarCofreRequest request, CancellationToken ct)
    {
        Enum.TryParse<CategoriaCofre>(request.Categoria, true, out var categoria);
        Enum.TryParse<VisibilidadeCofre>(request.Visibilidade, true, out var visibilidade);

        var cofre = await _mediator.Send(
            new CriarCofreCommand(request.Nome, request.Meta, UsuarioId, request.Descricao, categoria, visibilidade), ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = cofre.Id }, cofre);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarCofreRequest request, CancellationToken ct)
    {
        Enum.TryParse<CategoriaCofre>(request.Categoria, true, out var categoria);
        Enum.TryParse<VisibilidadeCofre>(request.Visibilidade, true, out var visibilidade);

        var cofre = await _mediator.Send(
            new AtualizarCofreCommand(id, request.Nome, request.Meta, request.Descricao, categoria, visibilidade), ct);
        return cofre is null ? NotFound() : Ok(cofre);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Deletar(Guid id, CancellationToken ct)
    {
        var deletado = await _mediator.Send(new DeletarCofreCommand(id), ct);
        return deletado ? NoContent() : NotFound();
    }

    /// <summary>Marca o cofre como concluído e notifica todos os participantes por e-mail.</summary>
    [HttpPut("{id:guid}/concluir")]
    public async Task<IActionResult> Concluir(Guid id, CancellationToken ct)
    {
        var cofre = await _mediator.Send(new ConcluirCofreCommand(id), ct);
        return cofre is null ? NotFound() : Ok(cofre);
    }

    // ── Participantes ──────────────────────────────────────────────────────────

    [HttpGet("{id:guid}/participantes")]
    public async Task<IActionResult> ListarParticipantes(Guid id, CancellationToken ct)
    {
        var cofre = await _mediator.Send(new ObterCofreComDetalhesQuery(id), ct);
        if (cofre is null) return NotFound();
        return Ok(cofre.Participantes);
    }

    [HttpPost("{id:guid}/participantes")]
    public async Task<IActionResult> AdicionarParticipante(
        Guid id, [FromBody] AdicionarParticipanteRequest request, CancellationToken ct)
    {
        var participante = await _mediator.Send(new AdicionarParticipanteCommand(id, request.UsuarioId), ct);
        return Ok(participante);
    }

    [HttpDelete("{id:guid}/participantes/{participanteId:guid}")]
    public async Task<IActionResult> RemoverParticipante(Guid id, Guid participanteId, CancellationToken ct)
    {
        var removido = await _mediator.Send(new RemoverParticipanteCommand(id, participanteId), ct);
        return removido ? NoContent() : NotFound();
    }

    [HttpPatch("{id:guid}/participantes/{participanteId:guid}/role")]
    public async Task<IActionResult> AlterarRole(
        Guid id, Guid participanteId, [FromBody] AlterarRoleRequest request, CancellationToken ct)
    {
        if (!Enum.TryParse<RoleParticipante>(request.Role, true, out var novoRole))
            return BadRequest("Role inválida. Use 'Admin' ou 'Contributor'.");

        var participante = await _mediator.Send(new PromoverParticipanteCommand(id, participanteId, novoRole), ct);
        return Ok(participante);
    }
}

