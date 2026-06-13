using Ledger.Domain.Base;

namespace Ledger.Domain.Entities;

public class ReceitaTemplateDomain : BaseDomain
{
    public string Nome { get; private set; } = string.Empty;
    public decimal Valor { get; private set; }
    public string? Descricao { get; private set; }
    public Guid UsuarioId { get; private set; }
    public bool Ativa { get; private set; }

    private ReceitaTemplateDomain(Guid id, string nome, decimal valor, string? descricao,
        Guid usuarioId, bool ativa, DateTime createdAt, DateTime? updatedAt)
    {
        Id = id; Nome = nome; Valor = valor; Descricao = descricao;
        UsuarioId = usuarioId; Ativa = ativa; CreatedAt = createdAt; UpdatedAt = updatedAt;
        Validate();
    }

    public static ReceitaTemplateDomain Criar(string nome, decimal valor, string? descricao, Guid usuarioId)
        => new(Guid.NewGuid(), nome, valor, descricao, usuarioId, true, DateTime.UtcNow, null);

    public static ReceitaTemplateDomain Reconstituir(Guid id, string nome, decimal valor, string? descricao,
        Guid usuarioId, bool ativa, DateTime createdAt, DateTime? updatedAt)
        => new(id, nome, valor, descricao, usuarioId, ativa, createdAt, updatedAt);

    public void Atualizar(string nome, decimal valor, string? descricao)
    {
        Nome = nome; Valor = valor; Descricao = descricao;
        UpdatedAt = DateTime.UtcNow;
        Validate();
    }

    public void Desativar() { Ativa = false; UpdatedAt = DateTime.UtcNow; }
    public void Ativar()    { Ativa = true;  UpdatedAt = DateTime.UtcNow; }

    protected override void Validate()
    {
        RuleFor(!string.IsNullOrWhiteSpace(Nome), nameof(Nome), "O nome é obrigatório.");
        RuleFor(Nome == null || Nome.Length <= 100, nameof(Nome), "O nome deve ter no máximo 100 caracteres.");
        RuleFor(Valor > 0, nameof(Valor), "O valor deve ser maior que zero.");
        RuleFor(Valor <= 999_999_999, nameof(Valor), "O valor da receita é inválido.");
        RuleFor(UsuarioId != Guid.Empty, nameof(UsuarioId), "O usuário é obrigatório.");
    }
}
