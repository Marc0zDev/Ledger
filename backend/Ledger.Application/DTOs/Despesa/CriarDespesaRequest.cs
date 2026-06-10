using System.ComponentModel.DataAnnotations;

namespace Ledger.Application.DTOs.Despesa;

public class CriarDespesaRequest
{
    [Required]
    [MaxLength(200)]
    public string   Descricao      { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal  Valor          { get; set; }

    public DateTime DataVencimento { get; set; }

    /// <summary>Categoria da despesa (padrão: 99 = Outro).</summary>
    public int Categoria   { get; set; } = 99;

    public bool Recorrente { get; set; } = false;
}
