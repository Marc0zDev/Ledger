namespace Ledger.Application.DTOs.Convite;

public class ConviteGrupoResponse
{
    public Guid   Id                    { get; set; }
    public Guid   GrupoId              { get; set; }
    public string GrupoNome            { get; set; } = string.Empty;
    public string Token                { get; set; } = string.Empty;
    public string Status               { get; set; } = string.Empty;
    public DateTime ExpiresAt          { get; set; }
    public DateTime CreatedAt          { get; set; }
}
