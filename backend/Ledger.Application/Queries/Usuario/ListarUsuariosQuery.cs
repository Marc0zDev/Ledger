using AutoMapper;
using Ledger.Application.DTOs.Usuario;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Queries.Usuario;

// ── Query ─────────────────────────────────────────────────────────────────────
public record ListarUsuariosQuery : IRequest<IEnumerable<UsuarioResponse>>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class ListarUsuariosQueryHandler : IRequestHandler<ListarUsuariosQuery, IEnumerable<UsuarioResponse>>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IMapper            _mapper;

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
