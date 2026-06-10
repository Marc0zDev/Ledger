namespace Ledger.Application.DTOs.Convite;

public class ConviteResponse
{
    public Guid   Id                    { get; set; }
    public Guid   CofreId               { get; set; }
    public string CofreNome             { get; set; } = string.Empty;
    public string Token                 { get; set; } = string.Empty;
    public string Status                { get; set; } = string.Empty;
    public DateTime ExpiresAt           { get; set; }
    public DateTime CreatedAt           { get; set; }
}
