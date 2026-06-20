using Ledger.Domain.Base;
using Ledger.Domain.Enums;

namespace Ledger.Domain.Entities;

public class GrupoMembroDomain : BaseDomain
{
    public Guid      GrupoId   { get; private set; }
    public Guid      UsuarioId { get; private set; }
    public RoleGrupo Role      { get; private set; } = RoleGrupo.Membro;

    public UsuarioDomain? Usuario { get; private set; }

    private GrupoMembroDomain() { }

    private GrupoMembroDomain(Guid grupoId, Guid usuarioId, RoleGrupo role)
    {
        GrupoId   = grupoId;
        UsuarioId = usuarioId;
        Role      = role;
        Validate();
    }

    private GrupoMembroDomain(Guid id, Guid grupoId, Guid usuarioId, RoleGrupo role, UsuarioDomain? usuario, DateTime createdAt, DateTime? updatedAt)
    {
        Id        = id;
        GrupoId   = grupoId;
        UsuarioId = usuarioId;
        Role      = role;
        Usuario   = usuario;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static GrupoMembroDomain Criar(Guid grupoId, Guid usuarioId, RoleGrupo role = RoleGrupo.Membro)
        => new(grupoId, usuarioId, role);

    public static GrupoMembroDomain Reconstituir(Guid id, Guid grupoId, Guid usuarioId, RoleGrupo role, UsuarioDomain? usuario, DateTime createdAt, DateTime? updatedAt)
        => new(id, grupoId, usuarioId, role, usuario, createdAt, updatedAt);

    protected override void Validate()
    {
        RuleFor(GrupoId   != Guid.Empty, nameof(GrupoId),   "O membro deve estar associado a um grupo.");
        RuleFor(UsuarioId != Guid.Empty, nameof(UsuarioId), "O membro deve estar associado a um usuário.");
    }
}
