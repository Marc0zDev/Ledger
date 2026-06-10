using AutoMapper;
using Ledger.Application.DTOs.Cofre;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Queries.Cofre;

public class ListarCofresQueryHandler : IRequestHandler<ListarCofresQuery, IEnumerable<CofreResponse>>
{
    private readonly ICofreRepository _cofreRepository;
    private readonly IMapper          _mapper;

    public ListarCofresQueryHandler(ICofreRepository cofreRepository, IMapper mapper)
    {
        _cofreRepository = cofreRepository;
        _mapper          = mapper;
    }

    public async Task<IEnumerable<CofreResponse>> Handle(ListarCofresQuery request, CancellationToken ct)
    {
        var cofres = await _cofreRepository.GetByUsuarioIdAsync(request.UsuarioId, ct);
        return _mapper.Map<IEnumerable<CofreResponse>>(cofres);
    }
}
