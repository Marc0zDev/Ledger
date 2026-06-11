namespace Ledger.Domain.Enums;

/// <summary>Visibilidade do cofre.</summary>
public enum VisibilidadeCofre
{
    /// <summary>Apenas o dono e participantes convidados enxergam o cofre.</summary>
    Privado = 1,

    /// <summary>Qualquer usuário pode visualizar o cofre (mas não pode movimentar sem convite).</summary>
    Compartilhado = 2,
}
