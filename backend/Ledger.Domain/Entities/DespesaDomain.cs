using Ledger.Domain.Base;
using Ledger.Domain.Enums;

namespace Ledger.Domain.Entities;

/// <summary>
/// Despesa pessoal do usuário — controle de contas mensais.
/// </summary>
public class DespesaDomain : BaseDomain
{
    public string            Descricao       { get; private set; } = string.Empty;
    public decimal           Valor           { get; private set; }
    public DateTime          DataVencimento  { get; private set; }
    public DateTime?         DataPagamento   { get; private set; }
    public bool              Paga            { get; private set; }
    public string?           BoletoPath      { get; private set; }
    public Guid              UsuarioId       { get; private set; }
    public CategoriaDespesa  Categoria       { get; private set; }
    public bool              Recorrente      { get; private set; }

    // ── Construtores ─────────────────────────────────────────────────────────

    private DespesaDomain(
        string           descricao,
        decimal          valor,
        DateTime         dataVencimento,
        Guid             usuarioId,
        CategoriaDespesa categoria,
        bool             recorrente)
    {
        Descricao      = descricao;
        Valor          = valor;
        DataVencimento = dataVencimento;
        UsuarioId      = usuarioId;
        Categoria      = categoria;
        Recorrente     = recorrente;
        Paga           = false;
        Validate();
    }

    private DespesaDomain(
        Guid             id,
        string           descricao,
        decimal          valor,
        DateTime         dataVencimento,
        DateTime?        dataPagamento,
        bool             paga,
        string?          boletoPath,
        Guid             usuarioId,
        CategoriaDespesa categoria,
        bool             recorrente,
        DateTime         createdAt,
        DateTime?        updatedAt)
    {
        Id             = id;
        Descricao      = descricao;
        Valor          = valor;
        DataVencimento = dataVencimento;
        DataPagamento  = dataPagamento;
        Paga           = paga;
        BoletoPath     = boletoPath;
        UsuarioId      = usuarioId;
        Categoria      = categoria;
        Recorrente     = recorrente;
        CreatedAt      = createdAt;
        UpdatedAt      = updatedAt;
    }

    // ── Factory methods ───────────────────────────────────────────────────────

    public static DespesaDomain Criar(
        string           descricao,
        decimal          valor,
        DateTime         dataVencimento,
        Guid             usuarioId,
        CategoriaDespesa categoria  = CategoriaDespesa.Outro,
        bool             recorrente = false)
        => new(descricao, valor, dataVencimento, usuarioId, categoria, recorrente);

    public static DespesaDomain Reconstituir(
        Guid             id,
        string           descricao,
        decimal          valor,
        DateTime         dataVencimento,
        DateTime?        dataPagamento,
        bool             paga,
        string?          boletoPath,
        Guid             usuarioId,
        CategoriaDespesa categoria,
        bool             recorrente,
        DateTime         createdAt,
        DateTime?        updatedAt)
        => new(id, descricao, valor, dataVencimento, dataPagamento, paga, boletoPath, usuarioId, categoria, recorrente, createdAt, updatedAt);

    // ── Comportamentos ────────────────────────────────────────────────────────

    public void Atualizar(string descricao, decimal valor, DateTime dataVencimento, CategoriaDespesa categoria, bool recorrente)
    {
        Descricao      = descricao;
        Valor          = valor;
        DataVencimento = dataVencimento;
        Categoria      = categoria;
        Recorrente     = recorrente;
        Validate();
        if (IsValid) MarkAsUpdated();
    }

    public void Pagar(DateTime? dataPagamento = null)
    {
        Paga          = true;
        DataPagamento = dataPagamento ?? DateTime.UtcNow;
        MarkAsUpdated();
    }

    public void AnexarBoleto(string path)
    {
        BoletoPath = path;
        MarkAsUpdated();
    }

    // ── Validação ─────────────────────────────────────────────────────────────

    protected override void Validate()
    {
        RuleFor(!string.IsNullOrWhiteSpace(Descricao), nameof(Descricao), "A descrição da despesa é obrigatória.");
        RuleFor(Valor > 0,                              nameof(Valor),     "O valor da despesa deve ser maior que zero.");
        RuleFor(UsuarioId != Guid.Empty,                nameof(UsuarioId), "A despesa deve estar associada a um usuário.");
        RuleFor(DataVencimento != default,              nameof(DataVencimento), "A data de vencimento é obrigatória.");
    }
}
