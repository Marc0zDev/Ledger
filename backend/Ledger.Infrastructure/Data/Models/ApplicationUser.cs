using Microsoft.AspNetCore.Identity;

namespace Ledger.Infrastructure.Data.Models;

/// <summary>
/// Usuário do sistema — estende IdentityUser com campos de negócio.
/// Armazenado no schema "auth".
/// </summary>
public class ApplicationUser : IdentityUser<Guid>
{
    public string Nome { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
