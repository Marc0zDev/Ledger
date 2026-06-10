using System.ComponentModel.DataAnnotations;

namespace Ledger.Application.DTOs.Cofre;

public class CriarCofreRequest
{
    [Required]
    public string   Nome      { get; set; } = string.Empty;
    public decimal  Meta      { get; set; }
    public string?  Descricao { get; set; }

    /// <summary>Ex: Viagem, Emergencia, Objetivo, Moradia, Lazer, Educacao, Saude, Outro</summary>
    public string Categoria { get; set; } = "Outro";
}
