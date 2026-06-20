using AutoMapper;
using Ledger.Application.DTOs.Receita;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Queries.Grupo;

// ── Query ─────────────────────────────────────────────────────────────────────
public record ListarReceitasGrupoQuery(Guid GrupoId, Guid UsuarioId, DateTime Competencia)
    : IRequest<IEnumerable<ReceitaResponse>>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class ListarReceitasGrupoQueryHandler
    : IRequestHandler<ListarReceitasGrupoQuery, IEnumerable<ReceitaResponse>>
{
    private readonly IGrupoRepository   _grupoRepo;
    private readonly IReceitaRepository _receitaRepo;
    private readonly IMapper            _mapper;

    public ListarReceitasGrupoQueryHandler(
        IGrupoRepository grupoRepo,
        IReceitaRepository receitaRepo,
        IMapper mapper)
    {
        _grupoRepo   = grupoRepo;
        _receitaRepo = receitaRepo;
        _mapper      = mapper;
    }

    public async Task<IEnumerable<ReceitaResponse>> Handle(
        ListarReceitasGrupoQuery query, CancellationToken ct)
    {
        var grupo = await _grupoRepo.GetComMembrosAsync(query.GrupoId, ct)
            ?? throw new DomainValidationException(["Grupo não encontrado."]);

        if (grupo.Membros.All(m => m.UsuarioId != query.UsuarioId))
            throw new DomainValidationException(["Você não é membro deste grupo."]);

        var nomesPorUsuario = grupo.Membros
            .Where(m => m.Usuario != null)
            .ToDictionary(m => m.UsuarioId, m => m.Usuario!.Nome);

        var receitas = await _receitaRepo.GetByGrupoAndCompetenciaAsync(query.GrupoId, query.Competencia, ct);

        return receitas.Select(r =>
        {
            var response = _mapper.Map<ReceitaResponse>(r);
            if (nomesPorUsuario.TryGetValue(r.UsuarioId, out var nome))
                response.UsuarioNome = nome;
            return response;
        });
    }
}
