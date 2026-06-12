using System.Security.Claims;
using Ledger.Application.Commands.DespesaPeriodo;
using Ledger.Application.DTOs.DespesaPeriodo;
using Ledger.Application.Queries.DespesaPeriodo;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ledger.Api.Controllers;

/// <summary>
/// Lançamentos mensais de despesa (instâncias por competência).
/// </summary>
[ApiController]
[Route("api/despesas-periodo")]
[Authorize]
public class DespesasPeriodoController : ControllerBase
{
    private readonly IMediator _mediator;
    public DespesasPeriodoController(IMediator mediator) => _mediator = mediator;

    private Guid UsuarioId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? throw new UnauthorizedAccessException());

    /// <summary>
    /// Lista os lançamentos de uma competência (ex: ?competencia=2026-06-01).
    /// Se não informado, usa o mês atual.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] DateTime? competencia, CancellationToken ct)
    {
        var comp = competencia ?? new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var lista = await _mediator.Send(new ListarDespesasPeriodoQuery(UsuarioId, comp), ct);
        return Ok(lista);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId(Guid id, CancellationToken ct)
    {
        var item = await _mediator.Send(new ObterDespesaPeriodoQuery(id), ct);
        return item is null ? NotFound() : Ok(item);
    }

    /// <summary>Cria um lançamento avulso ou vinculado a um template.</summary>
    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarDespesaPeriodoRequest request, CancellationToken ct)
    {
        var item = await _mediator.Send(
            new CriarDespesaPeriodoCommand(
                request.DespesaId, request.CategoriaId, UsuarioId,
                request.Descricao, request.ValorPlanejado, request.Competencia), ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = item.Id }, item);
    }

    /// <summary>Atualiza descrição, valor planejado ou categoria de um lançamento.</summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarDespesaPeriodoRequest request, CancellationToken ct)
    {
        var item = await _mediator.Send(
            new AtualizarDespesaPeriodoCommand(id, request.Descricao, request.ValorPlanejado, request.CategoriaId), ct);
        return item is null ? NotFound() : Ok(item);
    }

    /// <summary>Marca o lançamento como pago.</summary>
    [HttpPatch("{id:guid}/pagar")]
    public async Task<IActionResult> Pagar(Guid id, [FromBody] PagarDespesaPeriodoRequest? request, CancellationToken ct)
    {
        var item = await _mediator.Send(
            new PagarDespesaPeriodoCommand(id, request?.DataPagamento, request?.ValorRealizado), ct);
        return item is null ? NotFound() : Ok(item);
    }

    /// <summary>Faz upload do PDF do boleto para o lançamento.</summary>
    [HttpPost("{id:guid}/boleto")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<IActionResult> UploadBoleto(Guid id, IFormFile arquivo, CancellationToken ct)
    {
        if (arquivo is null || arquivo.Length == 0) return BadRequest("Arquivo não enviado.");
        if (!arquivo.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase))
            return BadRequest("Apenas arquivos PDF são aceitos.");

        var existente = await _mediator.Send(new ObterDespesaPeriodoQuery(id), ct);
        if (existente is null) return NotFound();

        var pastaDestino = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "boletos");
        Directory.CreateDirectory(pastaDestino);
        var nomeArquivo = $"{id}.pdf";
        var caminho     = Path.Combine(pastaDestino, nomeArquivo);

        await using var stream = new FileStream(caminho, FileMode.Create, FileAccess.Write);
        await arquivo.CopyToAsync(stream, ct);

        var result = await _mediator.Send(new AnexarBoletoPeriodoCommand(id, $"boletos/{nomeArquivo}"), ct);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>Faz upload do comprovante de pagamento para o lançamento.</summary>
    [HttpPost("{id:guid}/comprovante")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<IActionResult> UploadComprovante(Guid id, IFormFile arquivo, CancellationToken ct)
    {
        if (arquivo is null || arquivo.Length == 0) return BadRequest("Arquivo não enviado.");

        var permitidos = new[] { "application/pdf", "image/jpeg", "image/png", "image/webp" };
        if (!permitidos.Contains(arquivo.ContentType.ToLowerInvariant()))
            return BadRequest("Apenas PDF ou imagens (JPEG, PNG, WEBP) são aceitos.");

        using var ms    = new MemoryStream();
        await arquivo.CopyToAsync(ms, ct);
        var bytes     = ms.ToArray();
        var extensao  = Path.GetExtension(arquivo.FileName).TrimStart('.').ToLowerInvariant();

        var result = await _mediator.Send(
            new AnexarComprovantePeriodoCommand(id, arquivo.FileName, arquivo.ContentType, extensao, bytes), ct);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>Remove um lançamento.</summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Remover(Guid id, CancellationToken ct)
    {
        var ok = await _mediator.Send(new RemoverDespesaPeriodoCommand(id), ct);
        return ok ? NoContent() : NotFound();
    }

    /// <summary>
    /// Gera automaticamente os lançamentos do mês a partir dos templates ativos.
    /// Retorna os lançamentos criados (ignora os que já existem).
    /// </summary>
    [HttpPost("gerar")]
    public async Task<IActionResult> Gerar([FromQuery] DateTime? competencia, CancellationToken ct)
    {
        var comp   = competencia ?? new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var criados = await _mediator.Send(new GerarPeriodoCommand(UsuarioId, comp), ct);
        return Ok(criados);
    }
}
