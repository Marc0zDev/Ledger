using AutoMapper;
using Ledger.Application.DTOs.Receita;
using Ledger.Domain.Entities;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Commands.Receita;

// ── Command ───────────────────────────────────────────────────────────────────
public record CriarReceitaTemplateCommand(
    Guid    UsuarioId,
    string  Nome,
    decimal Valor,
    string? Descricao) : IRequest<ReceitaTemplateResponse>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class CriarReceitaTemplateCommandHandler : IRequestHandler<CriarReceitaTemplateCommand, ReceitaTemplateResponse>
{
    private readonly IReceitaTemplateRepository _repo;
    private readonly IMapper                    _mapper;

    public CriarReceitaTemplateCommandHandler(IReceitaTemplateRepository repo, IMapper mapper)
    {
        _repo   = repo;
        _mapper = mapper;
    }

    public async Task<ReceitaTemplateResponse> Handle(CriarReceitaTemplateCommand cmd, CancellationToken ct)
    {
        var template = ReceitaTemplateDomain.Criar(cmd.Nome, cmd.Valor, cmd.Descricao, cmd.UsuarioId);

        if (!template.IsValid)
            throw new DomainValidationException(template.Notifications.Select(n => n.Message));

        await _repo.AddAsync(template, ct);
        return _mapper.Map<ReceitaTemplateResponse>(template);
    }
}
