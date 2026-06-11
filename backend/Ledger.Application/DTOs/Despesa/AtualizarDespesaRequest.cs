namespace Ledger.Application.DTOs.Despesa;

/// <summary>Atualiza um template de despesa.</summary>
public class AtualizarDespesaRequest
{
    public string  Nome           { get; set; } = string.Empty;
    public int     Tipo           { get; set; } = 3;
    public decimal ValorPlanejado { get; set; }
    public Guid    CategoriaId    { get; set; }
    public int?    DiaVencimento  { get; set; }
}

public class PagarDespesaRequest
{
    public DateTime? DataPagamento { get; set; }
}
