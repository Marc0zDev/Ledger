using AutoMapper;
using Ledger.Application.DTOs.Usuario;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Queries.Usuario;

// ── Query ─────────────────────────────────────────────────────────────────────
public record ObterUsuarioPorEmailQuery(string Email) : IRequest<UsuarioResponse?>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class ObterUsuarioPorEmailQueryHandler : IRequestHandler<ObterUsuarioPorEmailQuery, UsuarioResponse?>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IMapper            _mapper;

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
