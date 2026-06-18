using Ledger.Domain.Base;
using Ledger.Domain.Enums;

namespace Ledger.Domain.Entities;

public class ConviteGrupoDomain : BaseDomain
{
    public Guid          GrupoId              { get; private set; }
    public Guid          ConvidadoPorUsuarioId { get; private set; }
    public Guid          UsuarioId            { get; private set; }
    public string        Token                { get; private set; } = string.Empty;
    public ConviteStatus Status               { get; private set; }
    public DateTime      ExpiresAt            { get; private set; }

    private ConviteGrupoDomain(Guid grupoId, Guid convidadoPorUsuarioId, Guid usuarioId)
    {
        GrupoId               = grupoId;
        ConvidadoPorUsuarioId = convidadoPorUsuarioId;
        UsuarioId             = usuarioId;
        Token                 = Guid.NewGuid().ToString("N");
        Status                = ConviteStatus.Pendente;
        ExpiresAt             = DateTime.UtcNow.AddDays(7);
        Validate();
    }

    private ConviteGrupoDomain(Guid id, Guid grupoId, Guid convidadoPorUsuarioId, Guid usuarioId,
        string token, ConviteStatus status, DateTime expiresAt, DateTime createdAt, DateTime? updatedAt)
    {
        Id                    = id;
        GrupoId               = grupoId;
        ConvidadoPorUsuarioId = convidadoPorUsuarioId;
        UsuarioId             = usuarioId;
        Token                 = token;
        Status                = status;
        ExpiresAt             = expiresAt;
        CreatedAt             = createdAt;
        UpdatedAt             = updatedAt;
    }

    public static ConviteGrupoDomain Criar(Guid grupoId, Guid convidadoPorUsuarioId, Guid usuarioId)
        => new(grupoId, convidadoPorUsuarioId, usuarioId);

    public static ConviteGrupoDomain Reconstituir(Guid id, Guid grupoId, Guid convidadoPorUsuarioId, Guid usuarioId,
        string token, ConviteStatus status, DateTime expiresAt, DateTime createdAt, DateTime? updatedAt)
        => new(id, grupoId, convidadoPorUsuarioId, usuarioId, token, status, expiresAt, createdAt, updatedAt);

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
        RuleFor(GrupoId   != Guid.Empty, nameof(GrupoId),   "O convite deve estar associado a um grupo.");
        RuleFor(UsuarioId != Guid.Empty, nameof(UsuarioId), "O convite deve estar associado a um usuário.");
    }
}
