using Ledger.Domain.Base;
using Ledger.Domain.Enums;

namespace Ledger.Domain.Entities;

/// <summary>
/// Representa um lançamento financeiro (depósito ou retirada) no cofre.
/// Guarda o histórico de quem fez e quando.
/// </summary>
public class MovimentacaoDomain : BaseDomain
{
    public string            Descricao  { get; private set; } = string.Empty;
    public decimal           Valor      { get; private set; }
    public TipoMovimentacao  Tipo       { get; private set; }
    public DateTime          Data       { get; private set; }
    public Guid              CofreId    { get; private set; }
    public Guid              UsuarioId  { get; private set; }
    public string?           UsuarioNome { get; private set; }

    private MovimentacaoDomain() { }

    private MovimentacaoDomain(string descricao, decimal valor, TipoMovimentacao tipo, DateTime data, Guid cofreId, Guid usuarioId)
    {
        Descricao = descricao;
        Valor     = valor;
        Tipo      = tipo;
        Data      = data;
        CofreId   = cofreId;
        UsuarioId = usuarioId;
        Validate();
    }

    private MovimentacaoDomain(Guid id, string descricao, decimal valor, TipoMovimentacao tipo, DateTime data, Guid cofreId, Guid usuarioId, DateTime createdAt, DateTime? updatedAt)
    {
        Id        = id;
        Descricao = descricao;
        Valor     = valor;
        Tipo      = tipo;
        Data      = data;
        CofreId   = cofreId;
        UsuarioId = usuarioId;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static MovimentacaoDomain Criar(string descricao, decimal valor, TipoMovimentacao tipo, DateTime data, Guid cofreId, Guid usuarioId)
        => new(descricao, valor, tipo, data, cofreId, usuarioId);

    public static MovimentacaoDomain Reconstituir(Guid id, string descricao, decimal valor, TipoMovimentacao tipo, DateTime data, Guid cofreId, Guid usuarioId, DateTime createdAt, DateTime? updatedAt, string? usuarioNome = null)
        => new(id, descricao, valor, tipo, data, cofreId, usuarioId, createdAt, updatedAt) { UsuarioNome = usuarioNome };

    protected override void Validate()
    {
        if (string.IsNullOrWhiteSpace(Descricao))
            AddNotification(nameof(Descricao), "Descrição é obrigatória.");

        if (Valor <= 0)
            AddNotification(nameof(Valor), "O valor deve ser maior que zero.");

        if (CofreId == Guid.Empty)
            AddNotification(nameof(CofreId), "CofreId é obrigatório.");

        if (UsuarioId == Guid.Empty)
            AddNotification(nameof(UsuarioId), "UsuarioId é obrigatório.");
    }
}
