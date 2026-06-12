using System.Security.Claims;
using Ledger.Application.Commands.Arquivo;
using Ledger.Application.Commands.Despesa;
using Ledger.Application.DTOs.Arquivo;
using Ledger.Application.DTOs.Cofre;
using Ledger.Application.DTOs.Despesa;
using Ledger.Application.Queries.Despesa;
using Ledger.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ledger.Api.Controllers;

/// <summary>
/// Contas fixas — definições recorrentes que geram lançamentos mensais automaticamente.
/// </summary>
[ApiController]
[Route("api/despesas")]
[Authorize]
public class DespesasController : ControllerBase
{
    private readonly IMediator _mediator;

    public DespesasController(IMediator mediator) => _mediator = mediator;

    private Guid UsuarioId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? throw new UnauthorizedAccessException());

    /// <summary>Lista contas fixas do usuário com paginação.</summary>
    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] int page     = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new ListarDespesasQuery(UsuarioId, page, pageSize), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId(Guid id, CancellationToken ct)
    {
        var despesa = await _mediator.Send(new ObterDespesaQuery(id), ct);
        return despesa is null ? NotFound() : Ok(despesa);
    }

    /// <summary>Cria um novo template de despesa.</summary>
    [HttpPost]
    public async Task<IActionResult> Registrar([FromBody] CriarDespesaRequest request, CancellationToken ct)
    {
        var despesa = await _mediator.Send(
            new RegistrarDespesaCommand(
                request.Nome, (TipoDespesa)request.Tipo, request.ValorPlanejado,
                request.CategoriaId, UsuarioId, request.DiaVencimento), ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = despesa.Id }, despesa);
    }

    /// <summary>Atualiza um template de despesa.</summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarDespesaRequest request, CancellationToken ct)
    {
        var despesa = await _mediator.Send(
            new AtualizarDespesaCommand(id, request.Nome, (TipoDespesa)request.Tipo,
                request.ValorPlanejado, request.CategoriaId, request.DiaVencimento), ct);
        return despesa is null ? NotFound() : Ok(despesa);
    }

    [HttpPost("arquivo")]
    public async Task<IActionResult> AdionarBoleto([FromBody] ArquivoRequest request, CancellationToken ct)
    {
        var arquivoResponse = await _mediator.Send
        (
            new RegistrarArquivoCommand(
                request.DespesaID, 
                request.Nome, 
                request.Content, 
                request.ArquivoByte, 
                request.Extensao
                ), 
            ct
        );
        return arquivoResponse is null ? NotFound() : Ok(arquivoResponse);
    }

    /// <summary>Desativa um template (para de gerar lançamentos mensais).</summary>
    [HttpPatch("{id:guid}/desativar")]
    public async Task<IActionResult> Desativar(Guid id, CancellationToken ct)
    {
        var ok = await _mediator.Send(new DesativarDespesaCommand(id), ct);
        return ok ? NoContent() : NotFound();
    }

    /// <summary>Remove permanentemente um template.</summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Remover(Guid id, CancellationToken ct)
    {
        var ok = await _mediator.Send(new RemoverDespesaCommand(id), ct);
        return ok ? NoContent() : NotFound();
    }
}
