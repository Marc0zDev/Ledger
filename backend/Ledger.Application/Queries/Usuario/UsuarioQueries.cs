using AutoMapper;
using Ledger.Application.DTOs.Usuario;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Queries.Usuario;

// ── Listar ────────────────────────────────────────────────────────────────────
public record ListarUsuariosQuery : IRequest<IEnumerable<UsuarioResponse>>;

public class ListarUsuariosQueryHandler : IRequestHandler<ListarUsuariosQuery, IEnumerable<UsuarioResponse>>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IMapper           _mapper;

    public ListarUsuariosQueryHandler(IUsuarioRepository usuarioRepository, IMapper mapper)
    {
        _usuarioRepository = usuarioRepository;
        _mapper            = mapper;
    }

    public async Task<IEnumerable<UsuarioResponse>> Handle(ListarUsuariosQuery _, CancellationToken ct)
    {
        var usuarios = await _usuarioRepository.GetAllAsync(ct);
        return _mapper.Map<IEnumerable<UsuarioResponse>>(usuarios);
    }
}

// ── Obter por Id ──────────────────────────────────────────────────────────────
public record ObterUsuarioQuery(Guid Id) : IRequest<UsuarioResponse?>;

public class ObterUsuarioQueryHandler : IRequestHandler<ObterUsuarioQuery, UsuarioResponse?>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IMapper           _mapper;

    public ObterUsuarioQueryHandler(IUsuarioRepository usuarioRepository, IMapper mapper)
    {
        _usuarioRepository = usuarioRepository;
        _mapper            = mapper;
    }

    public async Task<UsuarioResponse?> Handle(ObterUsuarioQuery query, CancellationToken ct)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(query.Id, ct);
        return usuario is null ? null : _mapper.Map<UsuarioResponse>(usuario);
    }
}

// ── Obter por Email ───────────────────────────────────────────────────────────
public record ObterUsuarioPorEmailQuery(string Email) : IRequest<UsuarioResponse?>;

public class ObterUsuarioPorEmailQueryHandler : IRequestHandler<ObterUsuarioPorEmailQuery, UsuarioResponse?>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IMapper           _mapper;

    public ObterUsuarioPorEmailQueryHandler(IUsuarioRepository usuarioRepository, IMapper mapper)
    {
        _usuarioRepository = usuarioRepository;
        _mapper            = mapper;
    }

    public async Task<UsuarioResponse?> Handle(ObterUsuarioPorEmailQuery query, CancellationToken ct)
    {
        var usuario = await _usuarioRepository.GetByEmailAsync(query.Email, ct);
        return usuario is null ? null : _mapper.Map<UsuarioResponse>(usuario);
    }
}
