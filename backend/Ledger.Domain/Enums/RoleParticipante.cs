namespace Ledger.Domain.Enums;

/// <summary>Papel do membro dentro de um cofre.</summary>
public enum RoleParticipante
{
    /// <summary>Pode depositar, sacar, editar meta, convidar/remover membros e fechar o cofre.</summary>
    Admin = 1,

    /// <summary>Pode apenas depositar no cofre.</summary>
    Contributor = 2,
}
