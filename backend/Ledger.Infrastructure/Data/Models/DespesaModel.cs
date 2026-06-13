namespace Ledger.Infrastructure.Data.Models;

public class DespesaModel
{
    public Guid Id{ get; set; }
    public string Nome { get; set; } = string.Empty;
    public int Tipo { get; set; } 
    public decimal ValorPlanejado { get; set; }
    public int? DiaVencimento  { get; set; }
    public bool Ativa { get; set; } = true;
    public Guid? ArquivoId { get; set; }
    public Guid CategoriaId { get; set; }
    public Guid UsuarioId { get; set; }
    public DateTime  DataInicio { get; set; }
    public DateTime? DataFim    { get; set; }
    public DateTime  CreatedAt  { get; set; }
    public DateTime? UpdatedAt  { get; set; }

    // Navigation
    public CategoriaModel? Categoria { get; set; }
    public ArquivoModel? Arquivo { get; set; }
}
