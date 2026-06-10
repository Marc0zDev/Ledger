using Ledger.Domain.Base;

namespace Ledger.Application.Events;

/// <summary>
/// Despacha os domain events coletados por uma entidade para o MediatR.
/// Chamado pelos services após a persistência, para garantir que a transação
/// foi confirmada antes dos side-effects ocorrerem.
/// </summary>
public interface IDomainEventDispatcher
{
    Task DispatchAsync(BaseDomain entity, CancellationToken ct = default);
    Task DispatchAsync(IEnumerable<BaseDomain> entities, CancellationToken ct = default);
}
