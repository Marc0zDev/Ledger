using AutoMapper;
using Ledger.Application.DTOs.Movimentacao;
using Ledger.Domain.Entities;
using Ledger.Domain.Enums;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Commands.Movimentacao;

// ── Command ───────────────────────────────────────────────────────────────────
public record RegistrarMovimentacaoCommand(
    Guid    CofreId,
    Guid    UsuarioId,
    string  Descricao,
    decimal Valor,
    string  Tipo,
    DateTime Data) : IRequest<MovimentacaoResponse>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class RegistrarMovimentacaoCommandHandler : IRequestHandler<RegistrarMovimentacaoCommand, MovimentacaoResponse>
{
    private readonly ICofreRepository        _cofreRepository;
    private readonly IMovimentacaoRepository _movimentacaoRepository;
    private readonly IMapper                 _mapper;

    public RegistrarMovimentacaoCommandHandler(
        ICofreRepository cofreRepository,
        IMovimentacaoRepository movimentacaoRepository,
        IMapper mapper)
    {
        _cofreRepository        = cofreRepository;
        _movimentacaoRepository = movimentacaoRepository;
        _mapper                 = mapper;
    }

    public async Task<MovimentacaoResponse> Handle(RegistrarMovimentacaoCommand cmd, CancellationToken ct)
    {
        var cofre = await _cofreRepository.GetByIdAsync(cmd.CofreId, ct)
            ?? throw new DomainValidationException(["Cofre não encontrado."]);

        if (!Enum.TryParse<TipoMovimentacao>(cmd.Tipo, true, out var tipo))
            throw new DomainValidationException([$"Tipo de movimentação inválido: {cmd.Tipo}."]);

        var mov = MovimentacaoDomain.Criar(
            cmd.Descricao, cmd.Valor, tipo, cmd.Data, cmd.CofreId, cmd.UsuarioId);

        cofre.RegistrarMovimentacao(mov);

        if (!cofre.IsValid)
            throw new DomainValidationException(cofre.Notifications.Select(n => n.Message));

        await _movimentacaoRepository.AddAsync(mov, ct);
        return _mapper.Map<MovimentacaoResponse>(mov);
    }
}
