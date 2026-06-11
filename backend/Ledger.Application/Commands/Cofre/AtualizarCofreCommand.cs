using AutoMapper;
using Ledger.Application.DTOs.Cofre;
using Ledger.Domain.Enums;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Commands.Cofre;

// ── Command ───────────────────────────────────────────────────────────────────
public record AtualizarCofreCommand(
    Guid              Id,
    string            Nome,
    decimal           Meta,
    string?           Descricao,
    CategoriaCofre    Categoria,
    VisibilidadeCofre Visibilidade = VisibilidadeCofre.Privado) : IRequest<CofreResponse?>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class AtualizarCofreCommandHandler : IRequestHandler<AtualizarCofreCommand, CofreResponse?>
{
    private readonly ICofreRepository _cofreRepository;
    private readonly IMapper          _mapper;

    public AtualizarCofreCommandHandler(ICofreRepository cofreRepository, IMapper mapper)
    {
        _cofreRepository = cofreRepository;
        _mapper          = mapper;
    }

    public async Task<CofreResponse?> Handle(AtualizarCofreCommand cmd, CancellationToken ct)
    {
        var cofre = await _cofreRepository.GetByIdAsync(cmd.Id, ct);
        if (cofre is null) return null;

        cofre.Atualizar(cmd.Nome, cmd.Meta, cmd.Descricao, cmd.Categoria, cmd.Visibilidade);

        if (!cofre.IsValid)
            throw new DomainValidationException(cofre.Notifications.Select(n => n.Message));

        await _cofreRepository.UpdateAsync(cofre, ct);
        return _mapper.Map<CofreResponse>(cofre);
    }
}

