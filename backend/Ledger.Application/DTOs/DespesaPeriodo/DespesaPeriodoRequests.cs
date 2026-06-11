namespace Ledger.Application.DTOs.DespesaPeriodo;

/// <summary>Cria um lançamento avulso ou vinculado a um template para uma competência.</summary>
public class CriarDespesaPeriodoRequest
{
    /// <summary>Template de origem (null = lançamento avulso).</summary>
    public Guid?     DespesaId      { get; set; }
    public Guid      CategoriaId    { get; set; }
    public string    Descricao      { get; set; } = string.Empty;
    public decimal   ValorPlanejado { get; set; }

    /// <summary>Competência no formato YYYY-MM-01 (primeiro dia do mês, UTC).</summary>
    public DateTime  Competencia    { get; set; }
}

public class AtualizarDespesaPeriodoRequest
{
    public string  Descricao      { get; set; } = string.Empty;
    public decimal ValorPlanejado { get; set; }
    public Guid    CategoriaId    { get; set; }
}

public class PagarDespesaPeriodoRequest
{
    public DateTime? DataPagamento { get; set; }
    public decimal?  ValorRealizado { get; set; }
}
