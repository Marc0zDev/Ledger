namespace Ledger.Application.DTOs.Despesa;

public class AtualizarDespesaRequest
{
    public string    Nome           { get; set; } = string.Empty;
    public int       Tipo           { get; set; } = 3;
    public decimal   ValorPlanejado { get; set; }
    public Guid      CategoriaId    { get; set; }
    public DateTime  DataInicio     { get; set; }
    public DateTime? DataFim        { get; set; }
    public int?      DiaVencimento  { get; set; }
    public Guid?     GrupoId        { get; set; }
}

public class PagarDespesaRequest
{
    public DateTime? DataPagamento { get; set; }
}
