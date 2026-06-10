using Ledger.Application.DTOs.Usuario;
using Ledger.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ledger.Api.Controllers;

[ApiController]
[Route("api/usuarios")]
[Authorize]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public UsuariosController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken ct)
    {
        var usuarios = await _usuarioService.ListarAsync(ct);
        return Ok(usuarios);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId(Guid id, CancellationToken ct)
    {
        var usuario = await _usuarioService.ObterPorIdAsync(id, ct);
        return usuario is null ? NotFound() : Ok(usuario);
    }

    [HttpGet("email/{email}")]
    public async Task<IActionResult> ObterPorEmail(string email, CancellationToken ct)
    {
        var usuario = await _usuarioService.ObterPorEmailAsync(email, ct);
        return usuario is null ? NotFound() : Ok(usuario);
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarUsuarioRequest request, CancellationToken ct)
    {
        var usuario = await _usuarioService.CriarAsync(request, ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = usuario.Id }, usuario);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarUsuarioRequest request, CancellationToken ct)
    {
        var usuario = await _usuarioService.AtualizarAsync(id, request, ct);
        return usuario is null ? NotFound() : Ok(usuario);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Deletar(Guid id, CancellationToken ct)
    {
        var deletado = await _usuarioService.DeletarAsync(id, ct);
        return deletado ? NoContent() : NotFound();
    }
}
