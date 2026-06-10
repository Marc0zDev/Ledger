namespace Ledger.Application.DTOs.Cofre;

public class AtualizarCofreRequest
{
    public string   Nome      { get; set; } = string.Empty;
    public decimal  Meta      { get; set; }
    public string?  Descricao { get; set; }
    public string   Categoria { get; set; } = "Outro";
}
