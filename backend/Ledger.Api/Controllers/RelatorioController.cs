using System.Security.Claims;
using Ledger.Application.Queries.DespesaPeriodo;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ledger.Api.Controllers;

[ApiController]
[Route("api/relatorio")]
[Authorize]
public class RelatorioController : ControllerBase
{
    private readonly IMediator _mediator;
    public RelatorioController(IMediator mediator) => _mediator = mediator;

    private Guid UsuarioId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? throw new UnauthorizedAccessException());

    /// <summary>
    /// Gera o relatório mensal em PDF para a competência informada.
    /// Ex: GET /api/relatorio/mensal?competencia=2026-06-01
    /// </summary>
    [HttpGet("mensal")]
    public async Task<IActionResult> Mensal([FromQuery] DateTime? competencia, CancellationToken ct)
    {
        var comp = competencia
            ?? new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var pdf = await _mediator.Send(new GerarRelatorioMensalQuery(UsuarioId, comp), ct);

        var nomeArquivo = $"relatorio-{comp:yyyy-MM}.pdf";
        return File(pdf, "application/pdf", nomeArquivo);
    }
}
