using AutoMapper;
using Ledger.Application.DTOs.Usuario;
using Ledger.Application.Interfaces;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Interfaces;

namespace Ledger.Application.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IIdentityService   _identityService;
    private readonly IMapper            _mapper;

    public UsuarioService(
        IUsuarioRepository usuarioRepository,
        IIdentityService identityService,
        IMapper mapper)
    {
        _usuarioRepository = usuarioRepository;
        _identityService   = identityService;
        _mapper            = mapper;
    }

    public async Task<IEnumerable<UsuarioResponse>> ListarAsync(CancellationToken ct = default)
    {
        var usuarios = await _usuarioRepository.GetAllAsync(ct);
        return _mapper.Map<IEnumerable<UsuarioResponse>>(usuarios);
    }

    public async Task<UsuarioResponse?> ObterPorIdAsync(Guid id, CancellationToken ct = default)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(id, ct);
        return usuario is null ? null : _mapper.Map<UsuarioResponse>(usuario);
    }

    public async Task<UsuarioResponse?> ObterPorEmailAsync(string email, CancellationToken ct = default)
    {
        var usuario = await _usuarioRepository.GetByEmailAsync(email, ct);
        return usuario is null ? null : _mapper.Map<UsuarioResponse>(usuario);
    }

    public async Task<UsuarioResponse> CriarAsync(CriarUsuarioRequest request, CancellationToken ct = default)
    {
        var (success, errors, userId) = await _identityService.CriarUsuarioAsync(
            request.Nome, request.Email, request.Senha, ct);

        if (!success)
            throw new DomainValidationException(errors);

        var usuario = await _usuarioRepository.GetByIdAsync(userId, ct)
            ?? throw new DomainValidationException(["Erro ao recuperar usuário criado."]);

        return _mapper.Map<UsuarioResponse>(usuario);
    }

    public async Task<UsuarioResponse?> AtualizarAsync(Guid id, AtualizarUsuarioRequest request, CancellationToken ct = default)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(id, ct);
        if (usuario is null) return null;

        var emailConflito = await _usuarioRepository.GetByEmailAsync(request.Email, ct);
        if (emailConflito is not null && emailConflito.Id != id)
            throw new DomainValidationException(["Já existe um usuário com este e-mail."]);

        usuario.Atualizar(request.Nome, request.Email);
        if (!usuario.IsValid)
            throw new DomainValidationException(usuario.Notifications.Select(n => n.Message));

        await _usuarioRepository.UpdateAsync(usuario, ct);
        return _mapper.Map<UsuarioResponse>(usuario);
    }

    public async Task<bool> DeletarAsync(Guid id, CancellationToken ct = default)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(id, ct);
        if (usuario is null) return false;

        await _usuarioRepository.DeleteAsync(id, ct);
        return true;
    }
}
