using Ledger.Domain.Base;

namespace Ledger.Domain.Entities;

/// <summary>
/// Instância mensal de uma despesa. Criada automaticamente para despesas Fixas/Variáveis
/// ao gerar o período, ou manualmente para lançamentos avulsos.
/// </summary>
public class DespesaPeriodoDomain : BaseDomain
{
    /// <summary>Template de origem. Null = lançamento avulso (sem template).</summary>
    public Guid?     DespesaId      { get; private set; }
    public Guid      CategoriaId    { get; private set; }
    public Guid      UsuarioId      { get; private set; }

    /// <summary>Descrição do lançamento (nome da despesa ou descrição manual).</summary>
    public string    Descricao      { get; private set; } = string.Empty;

    /// <summary>Valor esperado para o período.</summary>
    public decimal   ValorPlanejado { get; private set; }

    /// <summary>Valor efetivamente pago. Igual ao planejado quando pago sem ajuste.</summary>
    public decimal   ValorRealizado { get; private set; }

    /// <summary>Data/hora do pagamento. Null = não pago.</summary>
    public DateTime? PagaEm         { get; private set; }

    public bool      Paga           => PagaEm.HasValue;

    /// <summary>Caminho do PDF do boleto (opcional).</summary>
    public string?   BoletoPath     { get; private set; }

    /// <summary>Arquivo de comprovante de pagamento (opcional).</summary>
    public Guid?     ComprovanteId  { get; private set; }

    /// <summary>Primeiro dia do mês/ano ao qual este lançamento pertence (UTC).</summary>
    public DateTime  Competencia    { get; private set; }

    public Guid?     GrupoId        { get; private set; }

    private DespesaPeriodoDomain() { }

    private DespesaPeriodoDomain(
        Guid?    despesaId,
        Guid     categoriaId,
        Guid     usuarioId,
        string   descricao,
        decimal  valorPlanejado,
        DateTime competencia,
        Guid?    grupoId)
    {
        DespesaId      = despesaId;
        CategoriaId    = categoriaId;
        UsuarioId      = usuarioId;
        Descricao      = descricao;
        ValorPlanejado = valorPlanejado;
        ValorRealizado = 0;
        Competencia    = competencia;
        GrupoId        = grupoId;
        Validate();
    }

    private DespesaPeriodoDomain(
        Guid      id,
        Guid?     despesaId,
        Guid      categoriaId,
        Guid      usuarioId,
        string    descricao,
        decimal   valorPlanejado,
        decimal   valorRealizado,
        DateTime? pagaEm,
        string?   boletoPath,
        Guid?     comprovanteId,
        DateTime  competencia,
        DateTime  createdAt,
        DateTime? updatedAt,
        Guid?     grupoId)
    {
        Id             = id;
        DespesaId      = despesaId;
        CategoriaId    = categoriaId;
        UsuarioId      = usuarioId;
        Descricao      = descricao;
        ValorPlanejado = valorPlanejado;
        ValorRealizado = valorRealizado;
        PagaEm         = pagaEm;
        BoletoPath     = boletoPath;
        ComprovanteId  = comprovanteId;
        Competencia    = competencia;
        GrupoId        = grupoId;
        CreatedAt      = createdAt;
        UpdatedAt      = updatedAt;
    }

    // ── Factory ───────────────────────────────────────────────────────────────

    public static DespesaPeriodoDomain Criar(
        Guid?    despesaId,
        Guid     categoriaId,
        Guid     usuarioId,
        string   descricao,
        decimal  valorPlanejado,
        DateTime competencia,
        Guid?    grupoId = null)
        => new(despesaId, categoriaId, usuarioId, descricao, valorPlanejado, competencia, grupoId);

    public static DespesaPeriodoDomain Reconstituir(
        Guid      id,
        Guid?     despesaId,
        Guid      categoriaId,
        Guid      usuarioId,
        string    descricao,
        decimal   valorPlanejado,
        decimal   valorRealizado,
        DateTime? pagaEm,
        string?   boletoPath,
        Guid?     comprovanteId,
        DateTime  competencia,
        DateTime  createdAt,
        DateTime? updatedAt,
        Guid?     grupoId = null)
        => new(id, despesaId, categoriaId, usuarioId, descricao, valorPlanejado,
               valorRealizado, pagaEm, boletoPath, comprovanteId, competencia, createdAt, updatedAt, grupoId);

    // ── Comportamento ─────────────────────────────────────────────────────────

    public void Atualizar(string descricao, decimal valorPlanejado, Guid categoriaId)
    {
        Descricao      = descricao;
        ValorPlanejado = valorPlanejado;
        CategoriaId    = categoriaId;
        UpdatedAt      = DateTime.UtcNow;
        Validate();
    }

    public void Pagar(DateTime? dataPagamento = null, decimal? valorRealizado = null)
    {
        PagaEm         = dataPagamento ?? DateTime.UtcNow;
        ValorRealizado = valorRealizado ?? ValorPlanejado;
        UpdatedAt      = DateTime.UtcNow;
    }

    public void AnexarBoleto(string path)
    {
        BoletoPath = path;
        UpdatedAt  = DateTime.UtcNow;
    }

    public void AnexarComprovante(Guid arquivoId)
    {
        ComprovanteId = arquivoId;
        UpdatedAt     = DateTime.UtcNow;
    }

    protected override void Validate()
    {
        if (string.IsNullOrWhiteSpace(Descricao))
            AddNotification(nameof(Descricao), "Descrição é obrigatória.");
        if (Descricao?.Length > 200)
            AddNotification(nameof(Descricao), "Descrição deve ter no máximo 200 caracteres.");
        if (ValorPlanejado < 0)
            AddNotification(nameof(ValorPlanejado), "Valor planejado não pode ser negativo.");
        if (CategoriaId == Guid.Empty)
            AddNotification(nameof(CategoriaId), "Categoria é obrigatória.");
        if (UsuarioId == Guid.Empty)
            AddNotification(nameof(UsuarioId), "UsuarioId é obrigatório.");
    }
}
