using System.Security.Claims;
using Ledger.Application.Commands.Despesa;
using Ledger.Application.DTOs.Despesa;
using Ledger.Application.Queries.Despesa;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ledger.Api.Controllers;

/// <summary>
/// Gerencia despesas pessoais do usuário (contas mensais).
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

    /// <summary>Lista todas as despesas do usuário logado.</summary>
    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken ct)
    {
        var despesas = await _mediator.Send(new ListarDespesasQuery(UsuarioId), ct);
        return Ok(despesas);
    }

    /// <summary>Lista despesas pendentes (não pagas), com filtro opcional de vencimento.</summary>
    [HttpGet("pendentes")]
    public async Task<IActionResult> Pendentes([FromQuery] DateTime? vencimentoAte, CancellationToken ct)
    {
        var despesas = await _mediator.Send(new ListarDespesasPendentesQuery(UsuarioId, vencimentoAte), ct);
        return Ok(despesas);
    }

    /// <summary>Lista despesas vinculadas a um cofre específico.</summary>
    [HttpGet("cofre/{cofreId:guid}")]
    public async Task<IActionResult> ListarPorCofre(Guid cofreId, CancellationToken ct)
    {
        return Ok(Array.Empty<object>());
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId(Guid id, CancellationToken ct)
    {
        var despesa = await _mediator.Send(new ObterDespesaQuery(id), ct);
        return despesa is null ? NotFound() : Ok(despesa);
    }

    /// <summary>Registra uma nova despesa pessoal.</summary>
    [HttpPost]
    public async Task<IActionResult> Registrar([FromBody] CriarDespesaRequest request, CancellationToken ct)
    {
        var despesa = await _mediator.Send(
            new RegistrarDespesaCommand(
                request.Descricao, request.Valor, request.DataVencimento,
                UsuarioId, (Domain.Enums.CategoriaDespesa)request.Categoria, request.Recorrente), ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = despesa.Id }, despesa);
    }

    /// <summary>Atualiza os dados de uma despesa.</summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarDespesaRequest request, CancellationToken ct)
    {
        var despesa = await _mediator.Send(
            new AtualizarDespesaCommand(id, request.Descricao, request.Valor, request.DataVencimento,
                (Domain.Enums.CategoriaDespesa)request.Categoria, request.Recorrente), ct);
        return despesa is null ? NotFound() : Ok(despesa);
    }

    /// <summary>Marca a despesa como paga.</summary>
    [HttpPatch("{id:guid}/pagar")]
    public async Task<IActionResult> Pagar(Guid id, [FromBody] PagarDespesaRequest? request, CancellationToken ct)
    {
        var despesa = await _mediator.Send(new PagarDespesaCommand(id, request?.DataPagamento), ct);
        return despesa is null ? NotFound() : Ok(despesa);
    }

    /// <summary>Faz upload do PDF do boleto vinculado a esta despesa.</summary>
    [HttpPost("{id:guid}/boleto")]
    [RequestSizeLimit(10 * 1024 * 1024)] // 10 MB
    public async Task<IActionResult> UploadBoleto(Guid id, IFormFile arquivo, CancellationToken ct)
    {
        if (arquivo is null || arquivo.Length == 0)
            return BadRequest("Arquivo não enviado.");

        if (!arquivo.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase))
            return BadRequest("Apenas arquivos PDF são aceitos.");

        var despesa = await _mediator.Send(new ObterDespesaQuery(id), ct);
        if (despesa is null) return NotFound();

        // Armazena em wwwroot/boletos/{id}.pdf
        var pastaDestino = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "boletos");
        Directory.CreateDirectory(pastaDestino);

        var nomeArquivo = $"{id}.pdf";
        var caminho     = Path.Combine(pastaDestino, nomeArquivo);

        await using var stream = new FileStream(caminho, FileMode.Create, FileAccess.Write);
        await arquivo.CopyToAsync(stream, ct);

        var result = await _mediator.Send(
            new AnexarBoletoCommand(id, $"boletos/{nomeArquivo}"), ct);

        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Remover(Guid id, CancellationToken ct)
    {
        var removido = await _mediator.Send(new RemoverDespesaCommand(id), ct);
        return removido ? NoContent() : NotFound();
    }
}

