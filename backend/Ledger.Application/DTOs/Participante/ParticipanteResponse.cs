namespace Ledger.Application.DTOs.Participante;

public class ParticipanteResponse
{
    public Guid Id { get; set; }
    public Guid CofreId { get; set; }
    public Guid UsuarioId { get; set; }
    public string Role { get; set; } = string.Empty;
    // Denormalizado do Usuario para facilitar o frontend
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
