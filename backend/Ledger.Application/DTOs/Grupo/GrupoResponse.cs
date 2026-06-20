namespace Ledger.Application.DTOs.Grupo;

public class GrupoMembroResponse
{
    public Guid      Id        { get; set; }
    public Guid      GrupoId   { get; set; }
    public Guid      UsuarioId { get; set; }
    public string    Nome      { get; set; } = string.Empty;
    public string    Email     { get; set; } = string.Empty;
    public string    Role      { get; set; } = string.Empty;
    public DateTime  CreatedAt { get; set; }
}

public class GrupoResponse
{
    public Guid      Id                 { get; set; }
    public string    Nome               { get; set; } = string.Empty;
    public string?   Descricao          { get; set; }
    public Guid      CriadoPorUsuarioId { get; set; }
    public DateTime  CreatedAt          { get; set; }
    public DateTime? UpdatedAt          { get; set; }
    public List<GrupoMembroResponse> Membros { get; set; } = new();
}
