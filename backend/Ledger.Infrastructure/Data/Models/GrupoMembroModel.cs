namespace Ledger.Infrastructure.Data.Models;

public class GrupoMembroModel
{
    public Guid      Id        { get; set; }
    public Guid      GrupoId   { get; set; }
    public Guid      UsuarioId { get; set; }
    public int       Role      { get; set; }
    public DateTime  CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public GrupoModel?      Grupo   { get; set; }
    public ApplicationUser? Usuario { get; set; }
}
