using AutoMapper;
using Ledger.Application.DTOs.Usuario;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Commands.Usuario;

// ── Command ───────────────────────────────────────────────────────────────────
public record AtualizarUsuarioCommand(Guid Id, string Nome, string Email) : IRequest<UsuarioResponse?>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class AtualizarUsuarioCommandHandler : IRequestHandler<AtualizarUsuarioCommand, UsuarioResponse?>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IMapper            _mapper;

    public AtualizarUsuarioCommandHandler(IUsuarioRepository usuarioRepository, IMapper mapper)
    {
        _usuarioRepository = usuarioRepository;
        _mapper            = mapper;
    }

    public async Task<UsuarioResponse?> Handle(AtualizarUsuarioCommand cmd, CancellationToken ct)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(cmd.Id, ct);
        if (usuario is null) return null;

        var emailConflito = await _usuarioRepository.GetByEmailAsync(cmd.Email, ct);
        if (emailConflito is not null && emailConflito.Id != cmd.Id)
            throw new DomainValidationException(["Já existe um usuário com este e-mail."]);

        usuario.Atualizar(cmd.Nome, cmd.Email);
        if (!usuario.IsValid)
            throw new DomainValidationException(usuario.Notifications.Select(n => n.Message));

        await _usuarioRepository.UpdateAsync(usuario, ct);
        return _mapper.Map<UsuarioResponse>(usuario);
    }
}
