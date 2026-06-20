namespace Ledger.Infrastructure.Data.Models;

public class DespesaPeriodoModel
{
    public Guid      Id             { get; set; }
    public Guid?     DespesaId      { get; set; }
    public Guid      CategoriaId    { get; set; }
    public Guid      UsuarioId      { get; set; }
    public string    Descricao      { get; set; } = string.Empty;
    public decimal   ValorPlanejado { get; set; }
    public decimal   ValorRealizado { get; set; }
    public DateTime? PagaEm         { get; set; }
    public string?   BoletoPath     { get; set; }
    public Guid?     ComprovanteId  { get; set; }
    public DateTime  Competencia    { get; set; }
    public Guid?     GrupoId        { get; set; }
    public DateTime  CreatedAt      { get; set; }
    public DateTime? UpdatedAt      { get; set; }

    // Navigation properties
    public DespesaModel?   Despesa   { get; set; }
    public CategoriaModel? Categoria { get; set; }
    public GrupoModel?     Grupo     { get; set; }
}
