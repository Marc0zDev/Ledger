using System.Security.Claims;
using Ledger.Application.Commands.Grupo;
using Ledger.Application.DTOs.Grupo;
using Ledger.Application.Queries.Grupo;
using Ledger.Application.DTOs.Convite;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ledger.Api.Controllers;

[ApiController]
[Route("api/grupos")]
[Authorize]
public class GruposController : ControllerBase
{
    private readonly IMediator _mediator;

    public GruposController(IMediator mediator) => _mediator = mediator;

    private Guid UsuarioId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? throw new UnauthorizedAccessException());

    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken ct)
    {
        var grupos = await _mediator.Send(new ListarGruposQuery(UsuarioId), ct);
        return Ok(grupos);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId(Guid id, CancellationToken ct)
    {
        var grupo = await _mediator.Send(new ObterGrupoQuery(id), ct);
        return grupo is null ? NotFound() : Ok(grupo);
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarGrupoRequest request, CancellationToken ct)
    {
        var grupo = await _mediator.Send(new CriarGrupoCommand(request.Nome, request.Descricao, UsuarioId), ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = grupo.Id }, grupo);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarGrupoRequest request, CancellationToken ct)
    {
        var grupo = await _mediator.Send(new AtualizarGrupoCommand(id, request.Nome, request.Descricao, UsuarioId), ct);
        return Ok(grupo);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Deletar(Guid id, CancellationToken ct)
    {
        var deletado = await _mediator.Send(new DeletarGrupoCommand(id, UsuarioId), ct);
        return deletado ? NoContent() : NotFound();
    }

    // ── Membros ────────────────────────────────────────────────────────────────

    [HttpPost("{id:guid}/membros")]
    public async Task<IActionResult> ConvidarMembro(Guid id, [FromBody] AdicionarMembroGrupoRequest request, CancellationToken ct)
    {
        var convite = await _mediator.Send(new EnviarConviteGrupoCommand(id, request.UsuarioId, UsuarioId), ct);
        return CreatedAtAction(nameof(ObterPorId), new { id }, convite);
    }

    [HttpDelete("{id:guid}/membros/{membroId:guid}")]
    public async Task<IActionResult> RemoverMembro(Guid id, Guid membroId, CancellationToken ct)
    {
        var removido = await _mediator.Send(new RemoverMembroGrupoCommand(id, membroId, UsuarioId), ct);
        return removido ? NoContent() : NotFound();
    }

    // ── Finanças ───────────────────────────────────────────────────────────────

    [HttpGet("{id:guid}/despesas")]
    public async Task<IActionResult> ListarDespesas(Guid id, [FromQuery] DateTime? competencia, CancellationToken ct)
    {
        var comp = competencia ?? new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var lista = await _mediator.Send(new ListarDespesasPeriodoGrupoQuery(id, UsuarioId, comp), ct);
        return Ok(lista);
    }

    [HttpGet("{id:guid}/receitas")]
    public async Task<IActionResult> ListarReceitas(Guid id, [FromQuery] DateTime? competencia, CancellationToken ct)
    {
        var comp = competencia ?? new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var lista = await _mediator.Send(new ListarReceitasGrupoQuery(id, UsuarioId, comp), ct);
        return Ok(lista);
    }
}
