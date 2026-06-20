namespace Ledger.Infrastructure.Data.Models;

public class CategoriaModel
{
    public Guid     Id        { get; set; }
    public Guid?    UsuarioId { get; set; }
    public string   Nome      { get; set; } = string.Empty;
    public string?  Icone     { get; set; }
    public string?  Cor       { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
