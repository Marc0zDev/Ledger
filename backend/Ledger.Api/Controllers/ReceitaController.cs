using Ledger.Application.Commands.Receita;
using Ledger.Application.DTOs.Receita;
using Ledger.Application.Queries.Receita;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ledger.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ReceitaController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReceitaController(IMediator mediator) => _mediator = mediator;

    // ── Receitas ──────────────────────────────────────────────────────────────

    [HttpGet("{usuarioId}")]
    public async Task<IActionResult> ListarPorUsuario(
        Guid usuarioId,
        [FromQuery] DateTime? competencia,
        CancellationToken ct)
    {
        var receitas = await _mediator.Send(new ListarReceitaQuery(usuarioId, competencia), ct);
        return Ok(receitas);
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] ReceitaRequest request, CancellationToken ct)
    {
        var id = await _mediator.Send(new CriarReceitaCommand(
            request.UsuarioId,
            request.Nome,
            request.Valor,
            request.Descricao,
            request.ArquivoId,
            request.DataRecebimento,
            request.ReceitaTemplateId,
            request.GrupoId), ct);

        return CreatedAtAction(nameof(ListarPorUsuario), new { usuarioId = request.UsuarioId }, new { Id = id });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Deletar(Guid id, CancellationToken ct)
    {
        var ok = await _mediator.Send(new DeletarReceitaCommand(id), ct);
        return ok ? NoContent() : NotFound();
    }

    // ── Templates ─────────────────────────────────────────────────────────────

    [HttpGet("templates/{usuarioId}")]
    public async Task<IActionResult> ListarTemplates(Guid usuarioId, CancellationToken ct)
    {
        var templates = await _mediator.Send(new ListarReceitaTemplatesQuery(usuarioId), ct);
        return Ok(templates);
    }

    [HttpPost("templates")]
    public async Task<IActionResult> CriarTemplate([FromBody] CriarReceitaTemplateRequest request, CancellationToken ct)
    {
        var template = await _mediator.Send(new CriarReceitaTemplateCommand(
            request.UsuarioId,
            request.Nome,
            request.Valor,
            request.Descricao,
            request.DataInicio,
            request.DataFim), ct);

        return CreatedAtAction(nameof(ListarTemplates), new { usuarioId = request.UsuarioId }, template);
    }

    [HttpPut("templates/{id}")]
    public async Task<IActionResult> AtualizarTemplate(Guid id, [FromBody] AtualizarReceitaTemplateRequest request, CancellationToken ct)
    {
        var template = await _mediator.Send(new AtualizarReceitaTemplateCommand(
            id,
            request.Nome,
            request.Valor,
            request.Descricao,
            request.DataInicio,
            request.DataFim), ct);

        return template is null ? NotFound() : Ok(template);
    }

    [HttpDelete("templates/{id}")]
    public async Task<IActionResult> DeletarTemplate(Guid id, CancellationToken ct)
    {
        var ok = await _mediator.Send(new DeletarReceitaTemplateCommand(id), ct);
        return ok ? NoContent() : NotFound();
    }

    [HttpPost("templates/gerar")]
    public async Task<IActionResult> GerarDoMes([FromBody] GerarReceitasMesRequest request, CancellationToken ct)
    {
        var criadas = await _mediator.Send(new GerarReceitasMesCommand(request.UsuarioId, request.Competencia), ct);
        return Ok(criadas);
    }
}
