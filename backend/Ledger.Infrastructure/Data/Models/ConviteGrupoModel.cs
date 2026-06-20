namespace Ledger.Infrastructure.Data.Models;

public class ConviteGrupoModel
{
    public Guid     Id                     { get; set; }
    public Guid     GrupoId                { get; set; }
    public Guid     ConvidadoPorUsuarioId   { get; set; }
    public Guid     UsuarioId              { get; set; }
    public string   Token                  { get; set; } = string.Empty;
    public int      Status                 { get; set; } = 1;
    public DateTime ExpiresAt              { get; set; }
    public DateTime  CreatedAt             { get; set; }
    public DateTime? UpdatedAt             { get; set; }

    public GrupoModel? Grupo { get; set; }
}
