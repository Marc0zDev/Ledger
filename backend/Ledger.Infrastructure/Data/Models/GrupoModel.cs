namespace Ledger.Infrastructure.Data.Models;

public class GrupoModel
{
    public Guid      Id                 { get; set; }
    public string    Nome               { get; set; } = string.Empty;
    public string?   Descricao          { get; set; }
    public Guid      CriadoPorUsuarioId { get; set; }
    public DateTime  CreatedAt          { get; set; }
    public DateTime? UpdatedAt          { get; set; }

    public ApplicationUser?         CriadoPor { get; set; }
    public ICollection<GrupoMembroModel> Membros { get; set; } = new List<GrupoMembroModel>();
}
