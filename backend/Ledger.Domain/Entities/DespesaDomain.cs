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
    public DateTime    DataInicio     { get; private set; }
    public DateTime?   DataFim        { get; private set; }
    public Guid?       GrupoId        { get; private set; }

    private DespesaDomain() { }

    private DespesaDomain(string nome, TipoDespesa tipo, decimal valorPlanejado,
        Guid categoriaId, Guid usuarioId, int? diaVencimento, DateTime dataInicio, DateTime? dataFim,
        Guid? grupoId)
    {
        Nome           = nome;
        Tipo           = tipo;
        ValorPlanejado = valorPlanejado;
        CategoriaId    = categoriaId;
        UsuarioId      = usuarioId;
        DiaVencimento  = diaVencimento;
        DataInicio     = dataInicio;
        DataFim        = dataFim;
        GrupoId        = grupoId;
        Ativa          = true;
        Validate();
    }

    private DespesaDomain(Guid id, string nome, TipoDespesa tipo, decimal valorPlanejado,
        int? diaVencimento, bool ativa, Guid? arquivoId, Guid categoriaId, Guid usuarioId,
        DateTime dataInicio, DateTime? dataFim, DateTime createdAt, DateTime? updatedAt,
        Guid? grupoId)
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
        DataInicio     = dataInicio;
        DataFim        = dataFim;
        GrupoId        = grupoId;
        CreatedAt      = createdAt;
        UpdatedAt      = updatedAt;
    }

    // ── Factory ───────────────────────────────────────────────────────────────

    public static DespesaDomain Criar(string nome, TipoDespesa tipo, decimal valorPlanejado,
        Guid categoriaId, Guid usuarioId, DateTime dataInicio, DateTime? dataFim = null,
        int? diaVencimento = null, Guid? grupoId = null)
        => new(nome, tipo, valorPlanejado, categoriaId, usuarioId, diaVencimento, dataInicio, dataFim, grupoId);

    public static DespesaDomain Reconstituir(Guid id, string nome, TipoDespesa tipo, decimal valorPlanejado,
        int? diaVencimento, bool ativa, Guid? arquivoId, Guid categoriaId, Guid usuarioId,
        DateTime dataInicio, DateTime? dataFim, DateTime createdAt, DateTime? updatedAt,
        Guid? grupoId = null)
        => new(id, nome, tipo, valorPlanejado, diaVencimento, ativa, arquivoId, categoriaId, usuarioId, dataInicio, dataFim, createdAt, updatedAt, grupoId);

    // ── Comportamento ─────────────────────────────────────────────────────────

    public void Atualizar(string nome, TipoDespesa tipo, decimal valorPlanejado,
        Guid categoriaId, DateTime dataInicio, DateTime? dataFim, int? diaVencimento,
        Guid? grupoId = null)
    {
        Nome           = nome;
        Tipo           = tipo;
        ValorPlanejado = valorPlanejado;
        CategoriaId    = categoriaId;
        DiaVencimento  = diaVencimento;
        DataInicio     = dataInicio;
        DataFim        = dataFim;
        GrupoId        = grupoId;
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
        if (DataInicio == default)
            AddNotification(nameof(DataInicio), "Data de início é obrigatória.");
        if (DataFim.HasValue && DataFim.Value < DataInicio)
            AddNotification(nameof(DataFim), "Data de fim não pode ser anterior à data de início.");
    }
}
