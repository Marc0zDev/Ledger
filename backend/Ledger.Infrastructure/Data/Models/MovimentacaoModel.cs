namespace Ledger.Infrastructure.Data.Models;

public class MovimentacaoModel
{
    public Guid     Id        { get; set; }
    public string   Descricao { get; set; } = string.Empty;
    public decimal  Valor     { get; set; }
    public int      Tipo      { get; set; }
    public int      Status    { get; set; }
    public DateTime Data      { get; set; }
    public Guid     CofreId   { get; set; }
    public Guid     UsuarioId { get; set; }
    public DateTime  CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public CofreModel? Cofre    { get; set; }
    public ApplicationUser? Usuario { get; set; }
}
