namespace Ledger.Application.DTOs.DespesaPeriodo;

/// <summary>Lançamento mensal de despesa (instância de um período).</summary>
public class DespesaPeriodoResponse
{
    public Guid      Id             { get; set; }
    public Guid?     DespesaId      { get; set; }
    public string    Descricao      { get; set; } = string.Empty;
    public Guid      CategoriaId    { get; set; }
    public string    CategoriaNome  { get; set; } = string.Empty;
    public string?   CategoriaIcone { get; set; }
    public string?   CategoriaCor   { get; set; }
    public Guid      UsuarioId      { get; set; }
    public decimal   ValorPlanejado { get; set; }
    public decimal   ValorRealizado { get; set; }
    public bool      Paga           { get; set; }
    public DateTime? PagaEm         { get; set; }
    public string?   BoletoUrl      { get; set; }
    public DateTime  Competencia    { get; set; }
    public DateTime  CreatedAt      { get; set; }
    public DateTime? UpdatedAt      { get; set; }
}
