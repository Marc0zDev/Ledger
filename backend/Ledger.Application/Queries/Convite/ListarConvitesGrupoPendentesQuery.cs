using Ledger.Application.DTOs.Convite;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Queries.Convite;

// ── Query ────────────────────────────────────────────────────────────────────
public record ListarConvitesGrupoPendentesQuery(Guid UsuarioId) : IRequest<IEnumerable<ConviteGrupoResponse>>;

// ── Handler ──────────────────────────────────────────────────────────────────
public class ListarConvitesGrupoPendentesQueryHandler
    : IRequestHandler<ListarConvitesGrupoPendentesQuery, IEnumerable<ConviteGrupoResponse>>
{
    private readonly IConviteGrupoRepository _conviteGrupoRepository;
    private readonly IGrupoRepository        _grupoRepository;

    public ListarConvitesGrupoPendentesQueryHandler(
        IConviteGrupoRepository conviteGrupoRepository,
        IGrupoRepository grupoRepository)
    {
        _conviteGrupoRepository = conviteGrupoRepository;
        _grupoRepository        = grupoRepository;
    }

    public async Task<IEnumerable<ConviteGrupoResponse>> Handle(
        ListarConvitesGrupoPendentesQuery request, CancellationToken ct)
    {
        var convites = await _conviteGrupoRepository.GetPendentesByUsuarioIdAsync(request.UsuarioId, ct);

        var responses = new List<ConviteGrupoResponse>();
        foreach (var c in convites)
        {
            var grupo = await _grupoRepository.GetByIdAsync(c.GrupoId, ct);
            responses.Add(new ConviteGrupoResponse
            {
                Id        = c.Id,
                GrupoId   = c.GrupoId,
                GrupoNome = grupo?.Nome ?? string.Empty,
                Token     = c.Token,
                Status    = c.Status.ToString(),
                ExpiresAt = c.ExpiresAt,
                CreatedAt = c.CreatedAt,
            });
        }

        return responses;
    }
}
