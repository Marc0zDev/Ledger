namespace Ledger.Application.DTOs.Despesa;

public class AtualizarDespesaRequest
{
    public string   Descricao      { get; set; } = string.Empty;
    public decimal  Valor          { get; set; }
    public DateTime DataVencimento { get; set; }
    public int      Categoria      { get; set; } = 99;
    public bool     Recorrente     { get; set; } = false;
}

public class PagarDespesaRequest
{
    public DateTime? DataPagamento { get; set; }
}
