using Ledger.Application.Commands.Usuario;
using Ledger.Application.DTOs.Usuario;
using Ledger.Application.Queries.Usuario;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ledger.Api.Controllers;

[ApiController]
[Route("api/usuarios")]
[Authorize]
public class UsuariosController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsuariosController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken ct)
    {
        var usuarios = await _mediator.Send(new ListarUsuariosQuery(), ct);
        return Ok(usuarios);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId(Guid id, CancellationToken ct)
    {
        var usuario = await _mediator.Send(new ObterUsuarioQuery(id), ct);
        return usuario is null ? NotFound() : Ok(usuario);
    }

    [HttpGet("email/{email}")]
    public async Task<IActionResult> ObterPorEmail(string email, CancellationToken ct)
    {
        var usuario = await _mediator.Send(new ObterUsuarioPorEmailQuery(email), ct);
        return usuario is null ? NotFound() : Ok(usuario);
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarUsuarioRequest request, CancellationToken ct)
    {
        var usuario = await _mediator.Send(new CriarUsuarioCommand(request.Nome, request.Email, request.Senha), ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = usuario.Id }, usuario);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarUsuarioRequest request, CancellationToken ct)
    {
        var usuario = await _mediator.Send(new AtualizarUsuarioCommand(id, request.Nome, request.Email), ct);
        return usuario is null ? NotFound() : Ok(usuario);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Deletar(Guid id, CancellationToken ct)
    {
        var deletado = await _mediator.Send(new DeletarUsuarioCommand(id), ct);
        return deletado ? NoContent() : NotFound();
    }
}
