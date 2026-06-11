using System.Security.Claims;
using Ledger.Application.Queries.Categoria;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ledger.Api.Controllers;

[ApiController]
[Route("api/categorias")]
[Authorize]
public class CategoriasController : ControllerBase
{
    private readonly IMediator _mediator;
    public CategoriasController(IMediator mediator) => _mediator = mediator;

    private Guid UsuarioId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? throw new UnauthorizedAccessException());

    /// <summary>Lista categorias do sistema + categorias criadas pelo usuário.</summary>
    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken ct)
    {
        var categorias = await _mediator.Send(new ListarCategoriasQuery(UsuarioId), ct);
        return Ok(categorias);
    }
}
