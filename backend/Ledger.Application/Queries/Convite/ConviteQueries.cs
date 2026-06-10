using Ledger.Application.DTOs.Convite;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Queries.Convite;

// ── Query ────────────────────────────────────────────────────────────────────
public record ListarConvitesPendentesQuery(Guid UsuarioId) : IRequest<IEnumerable<ConviteResponse>>;

// ── Handler ──────────────────────────────────────────────────────────────────
public class ListarConvitesPendentesQueryHandler
    : IRequestHandler<ListarConvitesPendentesQuery, IEnumerable<ConviteResponse>>
{
    private readonly IConviteRepository _conviteRepository;
    private readonly ICofreRepository   _cofreRepository;

    public ListarConvitesPendentesQueryHandler(
        IConviteRepository conviteRepository,
        ICofreRepository cofreRepository)
    {
        _conviteRepository = conviteRepository;
        _cofreRepository   = cofreRepository;
    }

    public async Task<IEnumerable<ConviteResponse>> Handle(
        ListarConvitesPendentesQuery request, CancellationToken ct)
    {
        var convites = await _conviteRepository.GetPendentesByUsuarioIdAsync(request.UsuarioId, ct);

        var responses = new List<ConviteResponse>();
        foreach (var c in convites)
        {
            var cofre = await _cofreRepository.GetByIdAsync(c.CofreId, ct);
            responses.Add(new ConviteResponse
            {
                Id        = c.Id,
                CofreId   = c.CofreId,
                CofreNome = cofre?.Nome ?? string.Empty,
                Token     = c.Token,
                Status    = c.Status.ToString(),
                ExpiresAt = c.ExpiresAt,
                CreatedAt = c.CreatedAt,
            });
        }

        return responses;
    }
}
