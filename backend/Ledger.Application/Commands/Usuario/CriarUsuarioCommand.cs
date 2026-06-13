using AutoMapper;
using Ledger.Application.DTOs.Usuario;
using Ledger.Application.Interfaces;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Commands.Usuario;

// ── Command ───────────────────────────────────────────────────────────────────
public record CriarUsuarioCommand(string Nome, string Email, string Senha) : IRequest<UsuarioResponse>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class CriarUsuarioCommandHandler : IRequestHandler<CriarUsuarioCommand, UsuarioResponse>
{
    private readonly IIdentityService   _identityService;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IMapper            _mapper;

    public CriarUsuarioCommandHandler(
        IIdentityService identityService,
        IUsuarioRepository usuarioRepository,
        IMapper mapper)
    {
        _identityService   = identityService;
        _usuarioRepository = usuarioRepository;
        _mapper            = mapper;
    }

    public async Task<UsuarioResponse> Handle(CriarUsuarioCommand cmd, CancellationToken ct)
    {
        var (success, errors, userId) = await _identityService.CriarUsuarioAsync(cmd.Nome, cmd.Email, cmd.Senha, ct);

        if (!success)
            throw new DomainValidationException(errors);

        var usuario = await _usuarioRepository.GetByIdAsync(userId, ct)
            ?? throw new DomainValidationException(["Erro ao recuperar usuário criado."]);

        return _mapper.Map<UsuarioResponse>(usuario);
    }
}
