using System.ComponentModel.DataAnnotations;

namespace Ledger.Application.DTOs.Participante;

public class AdicionarParticipanteRequest
{
    [Required]
    public Guid UsuarioId { get; set; }
}

public class AlterarRoleRequest
{
    /// <summary>Admin ou Contributor</summary>
    [Required]
    public string Role { get; set; } = "Contributor";
}
