using AutoMapper;
using Ledger.Application.DTOs.Usuario;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Queries.Usuario;

// ── Query ─────────────────────────────────────────────────────────────────────
public record ObterUsuarioQuery(Guid Id) : IRequest<UsuarioResponse?>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class ObterUsuarioQueryHandler : IRequestHandler<ObterUsuarioQuery, UsuarioResponse?>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IMapper            _mapper;

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
