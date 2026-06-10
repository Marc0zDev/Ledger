using Ledger.Domain.Base;

namespace Ledger.Domain.Entities;

/// <summary>
/// Representa a participação de um <see cref="UsuarioDomain"/> em um cofre.
/// É uma entidade de membership: liga um Usuário a um Cofre.
/// </summary>
public class ParticipanteDomain : BaseDomain
{
    public Guid CofreId { get; private set; }
    public Guid UsuarioId { get; private set; }

    // Navegação (preenchida pelo repositório ao reconstituir)
    public UsuarioDomain? Usuario { get; private set; }

    private ParticipanteDomain(Guid cofreId, Guid usuarioId)
    {
        CofreId = cofreId;
        UsuarioId = usuarioId;
        Validate();
    }

    private ParticipanteDomain(Guid id, Guid cofreId, Guid usuarioId, UsuarioDomain? usuario, DateTime createdAt, DateTime? updatedAt)
    {
        Id = id;
        CofreId = cofreId;
        UsuarioId = usuarioId;
        Usuario = usuario;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    /// <summary>Cria uma nova membership. Verifique IsValid antes de persistir.</summary>
    public static ParticipanteDomain Criar(Guid cofreId, Guid usuarioId)
        => new(cofreId, usuarioId);

    /// <summary>Reconstitui a partir de dados persistidos. Uso exclusivo da Infrastructure.</summary>
    public static ParticipanteDomain Reconstituir(Guid id, Guid cofreId, Guid usuarioId, UsuarioDomain? usuario, DateTime createdAt, DateTime? updatedAt)
        => new(id, cofreId, usuarioId, usuario, createdAt, updatedAt);

    protected override void Validate()
    {
        RuleFor(CofreId != Guid.Empty, nameof(CofreId), "O participante deve estar associado a um cofre.");
        RuleFor(UsuarioId != Guid.Empty, nameof(UsuarioId), "O participante deve estar associado a um usuário.");
    }
}
