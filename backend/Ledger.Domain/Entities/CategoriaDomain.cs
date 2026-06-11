using Ledger.Domain.Base;

namespace Ledger.Domain.Entities;

/// <summary>
/// Categoria de despesa. Pode ser do sistema (UsuarioId = null) ou criada pelo usuário.
/// </summary>
public class CategoriaDomain : BaseDomain
{
    public string  Nome      { get; private set; } = string.Empty;
    public string? Icone     { get; private set; }
    public string? Cor       { get; private set; }
    public Guid?   UsuarioId { get; private set; }

    /// <summary>true se é uma categoria do sistema (não editável/removível pelo usuário).</summary>
    public bool IsSystem => UsuarioId is null;

    private CategoriaDomain() { }

    private CategoriaDomain(Guid? usuarioId, string nome, string? icone, string? cor)
    {
        UsuarioId = usuarioId;
        Nome      = nome;
        Icone     = icone;
        Cor       = cor;
        Validate();
    }

    private CategoriaDomain(Guid id, Guid? usuarioId, string nome, string? icone, string? cor,
        DateTime createdAt, DateTime? updatedAt)
    {
        Id        = id;
        UsuarioId = usuarioId;
        Nome      = nome;
        Icone     = icone;
        Cor       = cor;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static CategoriaDomain Criar(string nome, Guid? usuarioId = null, string? icone = null, string? cor = null)
        => new(usuarioId, nome, icone, cor);

    public static CategoriaDomain Reconstituir(Guid id, Guid? usuarioId, string nome, string? icone, string? cor,
        DateTime createdAt, DateTime? updatedAt)
        => new(id, usuarioId, nome, icone, cor, createdAt, updatedAt);

    public void Atualizar(string nome, string? icone, string? cor)
    {
        Nome      = nome;
        Icone     = icone;
        Cor       = cor;
        UpdatedAt = DateTime.UtcNow;
        Validate();
    }

    protected override void Validate()
    {
        if (string.IsNullOrWhiteSpace(Nome))
            AddNotification(nameof(Nome), "Nome da categoria é obrigatório.");
        if (Nome?.Length > 100)
            AddNotification(nameof(Nome), "Nome da categoria deve ter no máximo 100 caracteres.");
    }
}
