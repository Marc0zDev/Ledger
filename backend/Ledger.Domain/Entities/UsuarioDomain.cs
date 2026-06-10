using Ledger.Domain.Base;

namespace Ledger.Domain.Entities;

/// <summary>
/// Projeção de domínio do usuário — dados de negócio sem credenciais.
/// A gestão de senha/identidade é responsabilidade da camada de Infrastructure (Identity).
/// </summary>
public class UsuarioDomain : BaseDomain
{
    public string Nome  { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;

    private UsuarioDomain(string nome, string email)
    {
        Nome  = nome;
        Email = email;
        Validate();
    }

    private UsuarioDomain(Guid id, string nome, string email, DateTime createdAt, DateTime? updatedAt)
    {
        Id        = id;
        Nome      = nome;
        Email     = email;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static UsuarioDomain Criar(string nome, string email)
        => new(nome, email);

    public static UsuarioDomain Reconstituir(Guid id, string nome, string email, DateTime createdAt, DateTime? updatedAt)
        => new(id, nome, email, createdAt, updatedAt);

    public void Atualizar(string nome, string email)
    {
        Nome  = nome;
        Email = email;
        Validate();
        if (IsValid) MarkAsUpdated();
    }

    protected override void Validate()
    {
        RuleFor(!string.IsNullOrWhiteSpace(Nome),  nameof(Nome),  "O nome do usuário é obrigatório.");
        RuleFor(!string.IsNullOrWhiteSpace(Email), nameof(Email), "O e-mail do usuário é obrigatório.");
        RuleFor(Email.Contains('@'),               nameof(Email), "O e-mail informado não é válido.");
    }
}

