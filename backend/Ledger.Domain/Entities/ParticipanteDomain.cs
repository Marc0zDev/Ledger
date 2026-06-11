using Ledger.Domain.Base;
using Ledger.Domain.Enums;

namespace Ledger.Domain.Entities;

/// <summary>
/// Representa a participa��o de um <see cref="UsuarioDomain"/> em um cofre.
/// � uma entidade de membership: liga um Usu�rio a um Cofre com um papel (role).
/// </summary>
public class ParticipanteDomain : BaseDomain
{
    public Guid              CofreId   { get; private set; }
    public Guid              UsuarioId { get; private set; }
    public RoleParticipante  Role      { get; private set; } = RoleParticipante.Contributor;

    // Navega��o (preenchida pelo reposit�rio ao reconstituir)
    public UsuarioDomain? Usuario { get; private set; }

    private ParticipanteDomain(Guid cofreId, Guid usuarioId, RoleParticipante role)
    {
        CofreId   = cofreId;
        UsuarioId = usuarioId;
        Role      = role;
        Validate();
    }

    private ParticipanteDomain(Guid id, Guid cofreId, Guid usuarioId, RoleParticipante role,
        UsuarioDomain? usuario, DateTime createdAt, DateTime? updatedAt)
    {
        Id        = id;
        CofreId   = cofreId;
        UsuarioId = usuarioId;
        Role      = role;
        Usuario   = usuario;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    /// <summary>Cria uma nova membership. Role padr�o: Contributor.</summary>
    public static ParticipanteDomain Criar(Guid cofreId, Guid usuarioId,
        RoleParticipante role = RoleParticipante.Contributor)
        => new(cofreId, usuarioId, role);

    /// <summary>Reconstitui a partir de dados persistidos. Uso exclusivo da Infrastructure.</summary>
    public static ParticipanteDomain Reconstituir(Guid id, Guid cofreId, Guid usuarioId,
        RoleParticipante role, UsuarioDomain? usuario, DateTime createdAt, DateTime? updatedAt)
        => new(id, cofreId, usuarioId, role, usuario, createdAt, updatedAt);

    public void PromoverAdmin()
    {
        Role = RoleParticipante.Admin;
        MarkAsUpdated();
    }

    public void RebaixarContributor()
    {
        Role = RoleParticipante.Contributor;
        MarkAsUpdated();
    }

    protected override void Validate()
    {
        RuleFor(CofreId   != Guid.Empty, nameof(CofreId),   "O participante deve estar associado a um cofre.");
        RuleFor(UsuarioId != Guid.Empty, nameof(UsuarioId), "O participante deve estar associado a um usu�rio.");
    }
}


/// <summary>
/// Representa a participa��o de um <see cref="UsuarioDomain"/> em um cofre.
/// � uma entidade de membership: liga um Usu�rio a um Cofre.
