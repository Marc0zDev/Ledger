using AutoMapper;
using Ledger.Application.DTOs.Movimentacao;
using Ledger.Domain.Enums;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Commands.Movimentacao;

// ── Command ───────────────────────────────────────────────────────────────────
public record AprovarMovimentacaoCommand(
    Guid CofreId,
    Guid MovimentacaoId,
    Guid UsuarioId) : IRequest<MovimentacaoResponse>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class AprovarMovimentacaoCommandHandler : IRequestHandler<AprovarMovimentacaoCommand, MovimentacaoResponse>
{
    private readonly IMovimentacaoRepository _movimentacaoRepository;
    private readonly IParticipanteRepository  _participanteRepository;
    private readonly ICofreRepository         _cofreRepository;
    private readonly IMapper                  _mapper;

    public AprovarMovimentacaoCommandHandler(
        IMovimentacaoRepository movimentacaoRepository,
        IParticipanteRepository participanteRepository,
        ICofreRepository cofreRepository,
        IMapper mapper)
    {
        _movimentacaoRepository = movimentacaoRepository;
        _participanteRepository  = participanteRepository;
        _cofreRepository         = cofreRepository;
        _mapper                  = mapper;
    }

    public async Task<MovimentacaoResponse> Handle(AprovarMovimentacaoCommand cmd, CancellationToken ct)
    {
        var participante = await _participanteRepository.GetByCofreIdAndUsuarioIdAsync(cmd.CofreId, cmd.UsuarioId, ct);
        var isAdmin = participante?.Role == RoleParticipante.Admin;

        if (!isAdmin)
            throw new DomainValidationException(["Apenas administradores do cofre podem aprovar movimentações."]);

        var mov = await _movimentacaoRepository.GetByIdAsync(cmd.MovimentacaoId, ct)
            ?? throw new DomainValidationException(["Movimentação não encontrada."]);

        if (mov.CofreId != cmd.CofreId)
            throw new DomainValidationException(["Movimentação não pertence a este cofre."]);

        mov.Aprovar();
        await _movimentacaoRepository.UpdateAsync(mov, ct);

        return _mapper.Map<MovimentacaoResponse>(mov);
    }
}
