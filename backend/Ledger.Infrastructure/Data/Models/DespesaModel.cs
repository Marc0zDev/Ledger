namespace Ledger.Infrastructure.Data.Models;

public class DespesaModel
{
    public Guid      Id             { get; set; }
    public string    Descricao      { get; set; } = string.Empty;
    public decimal   Valor          { get; set; }
    public DateTime  DataVencimento { get; set; }
    public DateTime? DataPagamento  { get; set; }
    public bool      Paga           { get; set; }
    public string?   BoletoPath     { get; set; }
    public Guid      UsuarioId      { get; set; }
    public int       Categoria      { get; set; }
    public bool      Recorrente     { get; set; }
    public DateTime  CreatedAt      { get; set; }
    public DateTime? UpdatedAt      { get; set; }
}
