using AutoMapper;
using Ledger.Application.DTOs.Receita;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Commands.Receita;

// ── Command ───────────────────────────────────────────────────────────────────
public record AtualizarReceitaTemplateCommand(
    Guid    Id,
    string  Nome,
    decimal Valor,
    string? Descricao) : IRequest<ReceitaTemplateResponse?>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class AtualizarReceitaTemplateCommandHandler : IRequestHandler<AtualizarReceitaTemplateCommand, ReceitaTemplateResponse?>
{
    private readonly IReceitaTemplateRepository _repo;
    private readonly IMapper                    _mapper;

    public AtualizarReceitaTemplateCommandHandler(IReceitaTemplateRepository repo, IMapper mapper)
    {
        _repo   = repo;
        _mapper = mapper;
    }

    public async Task<ReceitaTemplateResponse?> Handle(AtualizarReceitaTemplateCommand cmd, CancellationToken ct)
    {
        var template = await _repo.GetByIdAsync(cmd.Id, ct);
        if (template is null) return null;

        template.Atualizar(cmd.Nome, cmd.Valor, cmd.Descricao);

        if (!template.IsValid)
            throw new DomainValidationException(template.Notifications.Select(n => n.Message));

        await _repo.UpdateAsync(template, ct);
        return _mapper.Map<ReceitaTemplateResponse>(template);
    }
}
