namespace Ledger.Infrastructure.Data.Models;

public class ReceitaTemplateModel
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public string? Descricao { get; set; }
    public Guid UsuarioId { get; set; }
    public bool      Ativa      { get; set; }
    public DateTime  DataInicio { get; set; }
    public DateTime? DataFim    { get; set; }
    public DateTime  CreatedAt  { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public virtual ApplicationUser Usuario { get; set; } = null!;
}
