using Ledger.Domain.Base;

namespace Ledger.Domain.Entities;

public class ReceitaDomain : BaseDomain
{
    public string Nome { get; private set; } = string.Empty;
    public decimal Valor { get; private set; }
    public string? Descricao { get; private set; }
    public Guid? ArquivoId { get; private set; }
    public DateTime DataRecebimento { get; private set; }
    public DateTime Competencia { get; private set; }
    public Guid? ReceitaTemplateId { get; private set; }
    public Guid UsuarioId { get; private set; }

    private ReceitaDomain(Guid id, string nome, decimal valor, string? descricao, Guid? arquivoId,
        DateTime dataRecebimento, DateTime competencia, Guid? receitaTemplateId,
        Guid usuarioId, DateTime createdAt, DateTime? updatedAt)
    {
        Id = id; Nome = nome; Valor = valor; Descricao = descricao; ArquivoId = arquivoId;
        DataRecebimento = dataRecebimento; Competencia = competencia;
        ReceitaTemplateId = receitaTemplateId; UsuarioId = usuarioId;
        CreatedAt = createdAt; UpdatedAt = updatedAt;
        Validate();
    }

    public static ReceitaDomain Criar(string nome, decimal valor, string? descricao,
        Guid? arquivoId, DateTime dataRecebimento, Guid usuarioId, Guid? receitaTemplateId = null)
    {
        var competencia = new DateTime(dataRecebimento.Year, dataRecebimento.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        return new(Guid.NewGuid(), nome, valor, descricao, arquivoId,
            dataRecebimento, competencia, receitaTemplateId, usuarioId, DateTime.UtcNow, null);
    }

    public static ReceitaDomain Reconstituir(Guid id, string nome, decimal valor, string? descricao,
        Guid? arquivoId, DateTime dataRecebimento, DateTime competencia, Guid? receitaTemplateId,
        Guid usuarioId, DateTime createdAt, DateTime? updatedAt)
        => new(id, nome, valor, descricao, arquivoId, dataRecebimento, competencia,
               receitaTemplateId, usuarioId, createdAt, updatedAt);

    public void Atualizar(string nome, decimal valor, string? descricao, Guid? arquivoId, DateTime dataRecebimento)
    {
        Nome = nome; Valor = valor; Descricao = descricao; ArquivoId = arquivoId;
        DataRecebimento = dataRecebimento;
        Competencia = new DateTime(dataRecebimento.Year, dataRecebimento.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        UpdatedAt = DateTime.UtcNow;
        Validate();
    }

    protected override void Validate()
    {
        RuleFor(!string.IsNullOrWhiteSpace(Nome), nameof(Nome), "O nome da receita é obrigatório.");
        RuleFor(Nome == null || Nome.Length <= 100, nameof(Nome), "O nome da receita deve ter no máximo 100 caracteres.");
        RuleFor(Valor > 0, nameof(Valor), "O valor da receita deve ser maior que zero.");
        RuleFor(Valor <= 999_999_999, nameof(Valor), "O valor da receita é inválido.");
        RuleFor(DataRecebimento != default, nameof(DataRecebimento), "A data de recebimento é obrigatória.");
        RuleFor(UsuarioId != Guid.Empty, nameof(UsuarioId), "O usuário é obrigatório.");
    }
}
