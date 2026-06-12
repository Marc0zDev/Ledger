using Ledger.Domain.Base;
using Ledger.Domain.Enums;
using Ledger.Domain.Events;

namespace Ledger.Domain.Entities;

/// <summary>
/// Aggregate Root do domínio Ledger.
/// Representa um cofre financeiro compartilhado com uma meta, categoria e participantes.
/// Despesas são um módulo independente e podem opcionalmente referenciar um cofre.
/// </summary>
public class CofreDomain : BaseDomain
{
    public string           Nome               { get; private set; } = string.Empty;
    public string?          Descricao          { get; private set; }
    public decimal          Meta               { get; private set; }
    public CofreStatus      Status             { get; private set; } = CofreStatus.Aberto;
    public CategoriaCofre   Categoria          { get; private set; } = CategoriaCofre.Outro;
    public VisibilidadeCofre Visibilidade      { get; private set; } = VisibilidadeCofre.Privado;
    public Guid             CriadoPorUsuarioId { get; private set; }

    // Participantes vinculados ao cofre
    private readonly List<ParticipanteDomain> _participantes = new();
    public IReadOnlyCollection<ParticipanteDomain> Participantes => _participantes.AsReadOnly();

    // Movimentações financeiras do cofre (depósitos e retiradas)
    private readonly List<MovimentacaoDomain> _movimentacoes = new();
    public IReadOnlyCollection<MovimentacaoDomain> Movimentacoes => _movimentacoes.AsReadOnly();

    // ── Propriedades calculadas ───────────────────────────────────────────────

    /// <summary>Saldo líquido das movimentações (Entradas − Saídas).</summary>
    public decimal TotalMovimentado =>
        _movimentacoes.Sum(m => m.Tipo == TipoMovimentacao.Entrada ? m.Valor : -m.Valor);

    // ── Construtores ──────────────────────────────────────────────────────────

    private CofreDomain(string nome, decimal meta, string? descricao, CategoriaCofre categoria,
        VisibilidadeCofre visibilidade, Guid criadoPorUsuarioId)
    {
        Nome               = nome;
        Meta               = meta;
        Descricao          = descricao;
        Categoria          = categoria;
        Visibilidade       = visibilidade;
        Status             = CofreStatus.Aberto;
        CriadoPorUsuarioId = criadoPorUsuarioId;
        Validate();
        if (IsValid) RaiseDomainEvent(new CofreCriadoEvent(Id, criadoPorUsuarioId));
    }

    private CofreDomain(
        Guid             id,
        string           nome,
        decimal          meta,
        string?          descricao,
        CofreStatus      status,
        CategoriaCofre   categoria,
        VisibilidadeCofre visibilidade,
        Guid             criadoPorUsuarioId,
        DateTime         createdAt,
        DateTime?        updatedAt)
    {
        Id                 = id;
        Nome               = nome;
        Meta               = meta;
        Descricao          = descricao;
        Status             = status;
        Categoria          = categoria;
        Visibilidade       = visibilidade;
        CriadoPorUsuarioId = criadoPorUsuarioId;
        CreatedAt          = createdAt;
        UpdatedAt          = updatedAt;
    }

    // ── Factory Methods ───────────────────────────────────────────────────────

    public static CofreDomain Criar(
        string           nome,
        decimal          meta,
        Guid             criadoPorUsuarioId,
        string?          descricao    = null,
        CategoriaCofre   categoria    = CategoriaCofre.Outro,
        VisibilidadeCofre visibilidade = VisibilidadeCofre.Privado)
        => new(nome, meta, descricao, categoria, visibilidade, criadoPorUsuarioId);

    public static CofreDomain Reconstituir(
        Guid             id,
        string           nome,
        decimal          meta,
        string?          descricao,
        CofreStatus      status,
        CategoriaCofre   categoria,
        VisibilidadeCofre visibilidade,
        Guid             criadoPorUsuarioId,
        DateTime         createdAt,
        DateTime?        updatedAt,
        IEnumerable<ParticipanteDomain>? participantes = null,
        IEnumerable<MovimentacaoDomain>? movimentacoes = null)
    {
        var cofre = new CofreDomain(id, nome, meta, descricao, status, categoria, visibilidade, criadoPorUsuarioId, createdAt, updatedAt);
        if (participantes is not null) cofre._participantes.AddRange(participantes);
        if (movimentacoes is not null) cofre._movimentacoes.AddRange(movimentacoes);
        return cofre;
    }

    // ── Comportamentos ────────────────────────────────────────────────────────

    public void Atualizar(string nome, decimal meta, string? descricao, CategoriaCofre categoria,
        VisibilidadeCofre visibilidade = VisibilidadeCofre.Privado)
    {
        Nome         = nome;
        Meta         = meta;
        Descricao    = descricao;
        Categoria    = categoria;
        Visibilidade = visibilidade;
        Validate();
        if (IsValid) MarkAsUpdated();
    }

    public void AdicionarParticipante(ParticipanteDomain participante)
    {
        if (!participante.IsValid) { AddNotificationsFrom(participante); return; }

        if (_participantes.Any(p => p.UsuarioId == participante.UsuarioId))
        {
            AddNotification(nameof(Participantes), "Este usuário já é participante do cofre.");
            return;
        }

        _participantes.Add(participante);
        RaiseDomainEvent(new ParticipanteAdicionadoEvent(Id, participante.UsuarioId));
        MarkAsUpdated();
    }

    public void RegistrarMovimentacao(MovimentacaoDomain movimentacao)
    {
        if (Status != CofreStatus.Aberto)
        {
            AddNotification(nameof(Status), $"Não é possível registrar movimentações em um cofre com status '{Status}'.");
            return;
        }

        if (!movimentacao.IsValid) { AddNotificationsFrom(movimentacao); return; }

        _movimentacoes.Add(movimentacao);
        MarkAsUpdated();
    }

    public void Concluir()
    {
        if (Status != CofreStatus.Aberto)
        {
            AddNotification(nameof(Status), "Somente cofres Abertos podem ser concluídos.");
            return;
        }

        Status = CofreStatus.Concluido;
        RaiseDomainEvent(new CofreConcluídoEvent(Id));
        MarkAsUpdated();
    }

    public void Cancelar()
    {
        if (Status == CofreStatus.Concluido)
        {
            AddNotification(nameof(Status), "Não é possível cancelar um cofre já Concluído.");
            return;
        }

        Status = CofreStatus.Cancelado;
        MarkAsUpdated();
    }

    // ── Validação ─────────────────────────────────────────────────────────────
    protected override void Validate()
    {
        RuleFor(!string.IsNullOrWhiteSpace(Nome), nameof(Nome), "O nome do cofre é obrigatório.");
        RuleFor(Meta > 0, nameof(Meta), "A meta financeira do cofre deve ser maior que zero.");
    }
}

