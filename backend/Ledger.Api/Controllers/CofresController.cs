using System.Security.Claims;
using Ledger.Application.Commands.Cofre;
using Ledger.Application.DTOs.Cofre;
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

        var cofre = await _mediator.Send(
            new CriarCofreCommand(request.Nome, request.Meta, UsuarioId, request.Descricao, categoria), ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = cofre.Id }, cofre);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarCofreRequest request, CancellationToken ct)
    {
        Enum.TryParse<CategoriaCofre>(request.Categoria, true, out var categoria);

        var cofre = await _mediator.Send(
            new AtualizarCofreCommand(id, request.Nome, request.Meta, request.Descricao, categoria), ct);
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
}

