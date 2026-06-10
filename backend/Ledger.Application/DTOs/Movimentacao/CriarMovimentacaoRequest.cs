using System.ComponentModel.DataAnnotations;

namespace Ledger.Application.DTOs.Movimentacao;

public class CriarMovimentacaoRequest
{
    [Required]
    [MaxLength(200)]
    public string   Descricao { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero.")]
    public decimal  Valor     { get; set; }

    /// <summary>"Entrada" ou "Saida"</summary>
    [Required]
    public string   Tipo      { get; set; } = "Entrada";

    public DateTime Data      { get; set; } = DateTime.UtcNow;
}
