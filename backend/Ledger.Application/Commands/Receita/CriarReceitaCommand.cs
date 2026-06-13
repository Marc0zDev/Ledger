using Ledger.Domain.Entities;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Commands.Receita;

// ── Command ───────────────────────────────────────────────────────────────────
public record CriarReceitaCommand(
    Guid     UsuarioId,
    string   Nome,
    decimal  Valor,
    string?  Descricao,
    Guid?    ArquivoId,
    DateTime DataRecebimento,
    Guid?    ReceitaTemplateId = null) : IRequest<Guid>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class CriarReceitaCommandHandler : IRequestHandler<CriarReceitaCommand, Guid>
{
    private readonly IReceitaRepository _repo;

    public CriarReceitaCommandHandler(IReceitaRepository repo) => _repo = repo;

    public async Task<Guid> Handle(CriarReceitaCommand cmd, CancellationToken ct)
    {
        var dataRecebimento = DateTime.SpecifyKind(cmd.DataRecebimento, DateTimeKind.Utc);
        var receita = ReceitaDomain.Criar(cmd.Nome, cmd.Valor, cmd.Descricao, cmd.ArquivoId,
            dataRecebimento, cmd.UsuarioId, cmd.ReceitaTemplateId);

        if (!receita.IsValid)
            throw new DomainValidationException(receita.Notifications.Select(n => n.Message));

        await _repo.AddAsync(receita, ct);
        return receita.Id;
    }
}
