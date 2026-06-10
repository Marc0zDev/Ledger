namespace Ledger.Application.DTOs.Despesa;

public class DespesaResponse
{
    public Guid      Id             { get; set; }
    public string    Descricao      { get; set; } = string.Empty;
    public decimal   Valor          { get; set; }
    public DateTime  DataVencimento { get; set; }
    public DateTime? DataPagamento  { get; set; }
    public bool      Paga           { get; set; }
    public string?   BoletoUrl      { get; set; }
    public Guid      UsuarioId      { get; set; }
    public string    Categoria      { get; set; } = string.Empty;
    public bool      Recorrente     { get; set; }
    public DateTime  CreatedAt      { get; set; }
    public DateTime? UpdatedAt      { get; set; }
}
