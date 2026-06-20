using Ledger.Domain.Base;
using Ledger.Domain.Enums;

namespace Ledger.Domain.Entities;

public class GrupoDomain : BaseDomain
{
    public string  Nome               { get; private set; } = string.Empty;
    public string? Descricao          { get; private set; }
    public Guid    CriadoPorUsuarioId { get; private set; }

    private readonly List<GrupoMembroDomain> _membros = new();
    public IReadOnlyCollection<GrupoMembroDomain> Membros => _membros.AsReadOnly();

    private GrupoDomain() { }

    private GrupoDomain(string nome, string? descricao, Guid criadoPorUsuarioId)
    {
        Nome               = nome;
        Descricao          = descricao;
        CriadoPorUsuarioId = criadoPorUsuarioId;
        Validate();
    }

    private GrupoDomain(Guid id, string nome, string? descricao, Guid criadoPorUsuarioId, DateTime createdAt, DateTime? updatedAt)
    {
        Id                 = id;
        Nome               = nome;
        Descricao          = descricao;
        CriadoPorUsuarioId = criadoPorUsuarioId;
        CreatedAt          = createdAt;
        UpdatedAt          = updatedAt;
    }

    public static GrupoDomain Criar(string nome, string? descricao, Guid criadoPorUsuarioId)
        => new(nome, descricao, criadoPorUsuarioId);

    public static GrupoDomain Reconstituir(Guid id, string nome, string? descricao, Guid criadoPorUsuarioId, DateTime createdAt, DateTime? updatedAt, IEnumerable<GrupoMembroDomain>? membros = null)
    {
        var g = new GrupoDomain(id, nome, descricao, criadoPorUsuarioId, createdAt, updatedAt);
        if (membros is not null) g._membros.AddRange(membros);
        return g;
    }

    public void Atualizar(string nome, string? descricao)
    {
        Nome      = nome;
        Descricao = descricao;
        Validate();
        if (IsValid) MarkAsUpdated();
    }

    public void AdicionarMembro(GrupoMembroDomain membro)
    {
        if (!membro.IsValid) { AddNotificationsFrom(membro); return; }

        if (_membros.Any(m => m.UsuarioId == membro.UsuarioId))
        {
            AddNotification(nameof(Membros), "Este usuário já é membro do grupo.");
            return;
        }

        _membros.Add(membro);
        MarkAsUpdated();
    }

    protected override void Validate()
    {
        RuleFor(!string.IsNullOrWhiteSpace(Nome), nameof(Nome), "O nome do grupo é obrigatório.");
        RuleFor(CriadoPorUsuarioId != Guid.Empty, nameof(CriadoPorUsuarioId), "O grupo precisa de um criador.");
    }
}
