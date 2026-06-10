using Ledger.Domain.Base;
using Ledger.Domain.Enums;

namespace Ledger.Domain.Entities;

/// <summary>
/// Representa um convite pendente para um usuário participar de um cofre.
/// O participante só é criado após aceitar o convite.
/// </summary>
public class ConviteDomain : BaseDomain
{
    public Guid          CofreId              { get; private set; }
    public Guid          ConvidadoPorUsuarioId { get; private set; }
    public Guid          UsuarioId            { get; private set; }
    public string        Token                { get; private set; } = string.Empty;
    public ConviteStatus Status               { get; private set; }
    public DateTime      ExpiresAt            { get; private set; }

    private ConviteDomain(Guid cofreId, Guid convidadoPorUsuarioId, Guid usuarioId)
    {
        CofreId               = cofreId;
        ConvidadoPorUsuarioId = convidadoPorUsuarioId;
        UsuarioId             = usuarioId;
        Token                 = Guid.NewGuid().ToString("N");
        Status                = ConviteStatus.Pendente;
        ExpiresAt             = DateTime.UtcNow.AddDays(7);
        Validate();
    }

    private ConviteDomain(Guid id, Guid cofreId, Guid convidadoPorUsuarioId, Guid usuarioId,
        string token, ConviteStatus status, DateTime expiresAt, DateTime createdAt, DateTime? updatedAt)
    {
        Id                    = id;
        CofreId               = cofreId;
        ConvidadoPorUsuarioId = convidadoPorUsuarioId;
        UsuarioId             = usuarioId;
        Token                 = token;
        Status                = status;
        ExpiresAt             = expiresAt;
        CreatedAt             = createdAt;
        UpdatedAt             = updatedAt;
    }

    public static ConviteDomain Criar(Guid cofreId, Guid convidadoPorUsuarioId, Guid usuarioId)
        => new(cofreId, convidadoPorUsuarioId, usuarioId);

    public static ConviteDomain Reconstituir(Guid id, Guid cofreId, Guid convidadoPorUsuarioId, Guid usuarioId,
        string token, ConviteStatus status, DateTime expiresAt, DateTime createdAt, DateTime? updatedAt)
        => new(id, cofreId, convidadoPorUsuarioId, usuarioId, token, status, expiresAt, createdAt, updatedAt);

    public void Aceitar()
    {
        if (Status != ConviteStatus.Pendente)
            AddNotification(nameof(Status), "Este convite não está mais pendente.");
        if (DateTime.UtcNow > ExpiresAt)
            AddNotification(nameof(ExpiresAt), "Este convite expirou.");

        if (IsValid) Status = ConviteStatus.Aceito;
    }

    public void Recusar()
    {
        if (Status != ConviteStatus.Pendente)
            AddNotification(nameof(Status), "Este convite não está mais pendente.");
        if (IsValid) Status = ConviteStatus.Recusado;
    }

    protected override void Validate()
    {
        RuleFor(CofreId   != Guid.Empty, nameof(CofreId),    "O convite deve estar associado a um cofre.");
        RuleFor(UsuarioId != Guid.Empty, nameof(UsuarioId),  "O convite deve estar associado a um usuário.");
    }
}
