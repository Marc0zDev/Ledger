namespace Ledger.Application.DTOs.Despesa;

/// <summary>Cria um template de despesa.</summary>
public class CriarDespesaRequest
{
    public string  Nome           { get; set; } = string.Empty;
    public int     Tipo           { get; set; } = 3; // Avulsa default
    public decimal ValorPlanejado { get; set; }
    public Guid    CategoriaId    { get; set; }
    public int?    DiaVencimento  { get; set; }
}
