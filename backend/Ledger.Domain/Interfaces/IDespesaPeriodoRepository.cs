using Ledger.Domain.Entities;

namespace Ledger.Domain.Interfaces;

public interface IDespesaPeriodoRepository : IRepository<DespesaPeriodoDomain>
{
    /// <summary>Lista lançamentos do usuário para uma competência (mês/ano).</summary>
    Task<IEnumerable<DespesaPeriodoDomain>> GetByCompetenciaAsync(Guid usuarioId, DateTime competencia, CancellationToken ct = default);

    /// <summary>Lista todos os lançamentos não pagos do usuário.</summary>
    Task<IEnumerable<DespesaPeriodoDomain>> GetPendentesAsync(Guid usuarioId, CancellationToken ct = default);

    /// <summary>Verifica se já existem lançamentos gerados para um template em determinada competência.</summary>
    Task<bool> ExisteParaTemplateNoMesAsync(Guid despesaId, DateTime competencia, CancellationToken ct = default);
}
