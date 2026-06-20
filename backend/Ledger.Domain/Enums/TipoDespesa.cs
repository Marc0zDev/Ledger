namespace Ledger.Domain.Enums;

/// <summary>
/// Define o comportamento de uma despesa no ciclo mensal.
/// </summary>
public enum TipoDespesa
{
    /// <summary>Valor fixo todo mês. Gerada automaticamente a cada período.</summary>
    Fixa     = 1,
    /// <summary>Categoria com teto mensal. Gerada automaticamente; gastos lançados ao longo do mês.</summary>
    Variavel = 2,
    /// <summary>Eventual, sem recorrência. Lançada pontualmente.</summary>
    Avulsa   = 3,
}
