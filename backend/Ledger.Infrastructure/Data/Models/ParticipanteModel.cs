namespace Ledger.Infrastructure.Data.Models;

public class ParticipanteModel
{
    public Guid Id { get; set; }
    public Guid CofreId { get; set; }
    public Guid UsuarioId { get; set; }
    public int  Role { get; set; } = 2; // Contributor
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public CofreModel Cofre { get; set; } = null!;
    public ApplicationUser Usuario { get; set; } = null!;
}
