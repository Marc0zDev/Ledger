using AutoMapper;
using Ledger.Application.DTOs.Usuario;
using Ledger.Application.Interfaces;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Commands.Usuario;

// ── Criar ─────────────────────────────────────────────────────────────────────
public record CriarUsuarioCommand(string Nome, string Email, string Senha) : IRequest<UsuarioResponse>;

public class CriarUsuarioCommandHandler : IRequestHandler<CriarUsuarioCommand, UsuarioResponse>
{
    private readonly IIdentityService  _identityService;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IMapper           _mapper;

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

// ── Atualizar ─────────────────────────────────────────────────────────────────
public record AtualizarUsuarioCommand(Guid Id, string Nome, string Email) : IRequest<UsuarioResponse?>;

public class AtualizarUsuarioCommandHandler : IRequestHandler<AtualizarUsuarioCommand, UsuarioResponse?>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IMapper           _mapper;

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

// ── Deletar ───────────────────────────────────────────────────────────────────
public record DeletarUsuarioCommand(Guid Id) : IRequest<bool>;

public class DeletarUsuarioCommandHandler : IRequestHandler<DeletarUsuarioCommand, bool>
{
    private readonly IUsuarioRepository _usuarioRepository;

    public DeletarUsuarioCommandHandler(IUsuarioRepository usuarioRepository)
        => _usuarioRepository = usuarioRepository;

    public async Task<bool> Handle(DeletarUsuarioCommand cmd, CancellationToken ct)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(cmd.Id, ct);
        if (usuario is null) return false;

        await _usuarioRepository.DeleteAsync(cmd.Id, ct);
        return true;
    }
}
