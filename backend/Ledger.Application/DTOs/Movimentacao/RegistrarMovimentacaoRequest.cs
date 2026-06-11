using System.ComponentModel.DataAnnotations;

namespace Ledger.Application.DTOs.Movimentacao;

public class RegistrarMovimentacaoRequest
{
    [Required]
    public string  Descricao { get; set; } = string.Empty;
    public decimal Valor     { get; set; }
    /// <summary>Entrada ou Saida</summary>
    [Required]
    public string  Tipo      { get; set; } = "Entrada";
    public DateTime Data     { get; set; } = DateTime.UtcNow;
}
