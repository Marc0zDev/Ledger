using Ledger.Domain.Base;
using Ledger.Domain.Enums;

namespace Ledger.Domain.Entities;

/// <summary>
/// Template de despesa: define uma despesa recorrente ou variável que é clonada
/// automaticamente a cada período mensal. Despesas avulsas também podem ser criadas aqui
/// para depois serem lançadas pontualmente.
/// </summary>
public class DespesaDomain : BaseDomain
{
    public string      Nome           { get; private set; } = string.Empty;
    public TipoDespesa Tipo           { get; private set; }
    public decimal     ValorPlanejado { get; private set; }
    public int?        DiaVencimento  { get; private set; }
    public bool        Ativa          { get; private set; } = true;
    public Guid?       ArquivoId      { get; private set; }
    public Guid        CategoriaId    { get; private set; }
    public Guid        UsuarioId      { get; private set; }

    private DespesaDomain() { }

    private DespesaDomain(string nome, TipoDespesa tipo, decimal valorPlanejado,
        Guid categoriaId, Guid usuarioId, int? diaVencimento)
    {
        Nome           = nome;
        Tipo           = tipo;
        ValorPlanejado = valorPlanejado;
        CategoriaId    = categoriaId;
        UsuarioId      = usuarioId;
        DiaVencimento  = diaVencimento;
        Ativa          = true;
        Validate();
    }

    private DespesaDomain(Guid id, string nome, TipoDespesa tipo, decimal valorPlanejado,
        int? diaVencimento, bool ativa, Guid? arquivoId, Guid categoriaId, Guid usuarioId,
        DateTime createdAt, DateTime? updatedAt)
    {
        Id             = id;
        Nome           = nome;
        Tipo           = tipo;
        ValorPlanejado = valorPlanejado;
        DiaVencimento  = diaVencimento;
        Ativa          = ativa;
        ArquivoId      = arquivoId;
        CategoriaId    = categoriaId;
        UsuarioId      = usuarioId;
        CreatedAt      = createdAt;
        UpdatedAt      = updatedAt;
    }

    // ── Factory ───────────────────────────────────────────────────────────────

    public static DespesaDomain Criar(string nome, TipoDespesa tipo, decimal valorPlanejado,
        Guid categoriaId, Guid usuarioId, int? diaVencimento = null)
        => new(nome, tipo, valorPlanejado, categoriaId, usuarioId, diaVencimento);

    public static DespesaDomain Reconstituir(Guid id, string nome, TipoDespesa tipo, decimal valorPlanejado,
        int? diaVencimento, bool ativa, Guid? arquivoId, Guid categoriaId, Guid usuarioId,
        DateTime createdAt, DateTime? updatedAt)
        => new(id, nome, tipo, valorPlanejado, diaVencimento, ativa, arquivoId, categoriaId, usuarioId, createdAt, updatedAt);

    // ── Comportamento ─────────────────────────────────────────────────────────

    public void Atualizar(string nome, TipoDespesa tipo, decimal valorPlanejado,
        Guid categoriaId, int? diaVencimento)
    {
        Nome           = nome;
        Tipo           = tipo;
        ValorPlanejado = valorPlanejado;
        CategoriaId    = categoriaId;
        DiaVencimento  = diaVencimento;
        UpdatedAt      = DateTime.UtcNow;
        Validate();
    }

    public void Ativar()    { Ativa = true;  UpdatedAt = DateTime.UtcNow; }
    public void Desativar() { Ativa = false; UpdatedAt = DateTime.UtcNow; }

    public void AdicionarArquivo(Guid idarquivo) 
    { 
        if(idarquivo == null || idarquivo == Guid.Empty)
            AddNotification(nameof(ArquivoId), "Id do arquivo é obrigatório.");
        
        ArquivoId = idarquivo;
        UpdatedAt = DateTime.UtcNow;
    }

    // ── Validação ─────────────────────────────────────────────────────────────

    protected override void Validate()
    {
        if (string.IsNullOrWhiteSpace(Nome))
            AddNotification(nameof(Nome), "Nome da despesa é obrigatório.");
        if (Nome?.Length > 200)
            AddNotification(nameof(Nome), "Nome deve ter no máximo 200 caracteres.");
        if (ValorPlanejado < 0)
            AddNotification(nameof(ValorPlanejado), "Valor planejado não pode ser negativo.");
        if (CategoriaId == Guid.Empty)
            AddNotification(nameof(CategoriaId), "Categoria é obrigatória.");
        if (UsuarioId == Guid.Empty)
            AddNotification(nameof(UsuarioId), "UsuarioId é obrigatório.");
        if (DiaVencimento.HasValue && (DiaVencimento < 1 || DiaVencimento > 31))
            AddNotification(nameof(DiaVencimento), "Dia de vencimento deve ser entre 1 e 31.");
    }
}
