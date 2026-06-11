using System.Security.Claims;
using Ledger.Application.Commands.Movimentacao;
using Ledger.Application.DTOs.Movimentacao;
using Ledger.Application.Queries.Movimentacao;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ledger.Api.Controllers;

[ApiController]
[Route("api/cofres/{cofreId:guid}/movimentacoes")]
[Authorize]
public class MovimentacoesController : ControllerBase
{
    private readonly IMediator _mediator;

    public MovimentacoesController(IMediator mediator) => _mediator = mediator;

    private Guid UsuarioId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? throw new UnauthorizedAccessException());

    /// <summary>Lista movimentações de um cofre com paginação.</summary>
    [HttpGet]
    public async Task<IActionResult> Listar(
        Guid cofreId,
        [FromQuery] int page     = 1,
        [FromQuery] int pageSize = 5,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new ListarMovimentacoesQuery(cofreId, page, pageSize), ct);
        return Ok(result);
    }

    /// <summary>Registra uma nova movimentação (entrada ou saída) no cofre.</summary>
    [HttpPost]
    public async Task<IActionResult> Registrar(Guid cofreId, [FromBody] CriarMovimentacaoRequest request, CancellationToken ct)
    {
        var mov = await _mediator.Send(
            new RegistrarMovimentacaoCommand(cofreId, UsuarioId, request.Descricao, request.Valor, request.Tipo, request.Data), ct);
        return Created(string.Empty, mov);
    }
}
