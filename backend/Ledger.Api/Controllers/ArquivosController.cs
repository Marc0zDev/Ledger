using System.Security.Claims;
using Ledger.Application.Queries.Arquivo;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ledger.Api.Controllers;

[ApiController]
[Route("api/arquivos")]
[Authorize]
public class ArquivosController : ControllerBase
{
    private readonly IMediator _mediator;

    public ArquivosController(IMediator mediator) => _mediator = mediator;

    private Guid UsuarioId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? throw new UnauthorizedAccessException());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Obter(Guid id, CancellationToken ct)
    {
        var arquivo = await _mediator.Send(new ObterArquivoQuery(id, UsuarioId), ct);
        return arquivo is null
            ? NotFound()
            : File(arquivo.ArquivoByte, arquivo.ContentType, arquivo.Nome);
    }
}
