using Flunt.Notifications;
using Ledger.Domain.Events;

namespace Ledger.Domain.Base;

/// <summary>
/// Classe base para todas as entidades de domínio.
/// Estende <see cref="Notifiable{TNotification}"/> do Flunt com identidade, auditoria
/// e helpers de validação por regra.
/// </summary>
public abstract class BaseDomain : Notifiable<Notification>
{
    // ── Identidade e auditoria ────────────────────────────────────────────
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; protected set; }

    // ── Domain Events ─────────────────────────────────────────────────────
    private readonly List<IDomainEvent> _domainEvents = new();

    /// <summary>Eventos de domínio levantados por esta entidade. Despachados após persistência.</summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>Levanta um domain event a ser processado após a persistência.</summary>
    protected void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    /// <summary>Limpa a fila de eventos após o dispatch. Chamado pelo serviço/repositório.</summary>
    public void ClearDomainEvents() => _domainEvents.Clear();

    // ── Helpers de domínio ────────────────────────────────────────────────

    /// <summary>
    /// Propaga todas as notificações de outro <see cref="Notifiable{TNotification}"/> para este.
    /// </summary>
    protected void AddNotificationsFrom(Notifiable<Notification> other)
        => AddNotifications(other);

    /// <summary>
    /// Valida uma regra de negócio declarativamente.
    /// Se <paramref name="condition"/> for falsa, adiciona uma notificação de erro.
    /// </summary>
    protected void RuleFor(bool condition, string key, string message)
    {
        if (!condition)
            AddNotification(key, message);
    }

    /// <summary>Valida as regras de negócio da entidade. Deve ser implementado por cada domínio.</summary>
    protected abstract void Validate();

    /// <summary>Registra o timestamp da última alteração.</summary>
    protected void MarkAsUpdated() => UpdatedAt = DateTime.UtcNow;
}

