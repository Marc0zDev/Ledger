namespace Ledger.Infrastructure.Data.Models;

public class ConviteModel
{
    public Guid     Id                     { get; set; }
    public Guid     CofreId                { get; set; }
    public Guid     ConvidadoPorUsuarioId  { get; set; }
    public Guid     UsuarioId              { get; set; }
    public string   Token                  { get; set; } = string.Empty;
    public int      Status                 { get; set; } = 1; // Pendente
    public DateTime ExpiresAt              { get; set; }
    public DateTime  CreatedAt             { get; set; }
    public DateTime? UpdatedAt             { get; set; }

    // Navegação
    public CofreModel? Cofre { get; set; }
}
