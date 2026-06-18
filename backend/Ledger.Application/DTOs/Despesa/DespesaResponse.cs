namespace Ledger.Application.DTOs.Despesa;

/// <summary>Template de despesa (definição recorrente/fixa/variável).</summary>
public class DespesaResponse
{
    public Guid     Id             { get; set; }
    public string   Nome           { get; set; } = string.Empty;
    public string   Tipo           { get; set; } = string.Empty;
    public decimal  ValorPlanejado { get; set; }
    public int?     DiaVencimento  { get; set; }
    public bool      Ativa      { get; set; }
    public DateTime  DataInicio { get; set; }
    public DateTime? DataFim    { get; set; }
    public Guid?     ArquivoId  { get; set; }
    public Guid     CategoriaId    { get; set; }
    public string   CategoriaNome  { get; set; } = string.Empty;
    public string?  CategoriaIcone { get; set; }
    public string?  CategoriaCor   { get; set; }
    public Guid     UsuarioId      { get; set; }
    public Guid?    GrupoId        { get; set; }
    public DateTime CreatedAt      { get; set; }
    public DateTime? UpdatedAt     { get; set; }
}
