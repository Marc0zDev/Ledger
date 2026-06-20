using AutoMapper;
using Ledger.Application.DTOs.Movimentacao;
using Ledger.Domain.Enums;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Commands.Movimentacao;

// ── Command ───────────────────────────────────────────────────────────────────
public record RejeitarMovimentacaoCommand(
    Guid CofreId,
    Guid MovimentacaoId,
    Guid UsuarioId) : IRequest<MovimentacaoResponse>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class RejeitarMovimentacaoCommandHandler : IRequestHandler<RejeitarMovimentacaoCommand, MovimentacaoResponse>
{
    private readonly IMovimentacaoRepository _movimentacaoRepository;
    private readonly IParticipanteRepository  _participanteRepository;
    private readonly IMapper                  _mapper;

    public RejeitarMovimentacaoCommandHandler(
        IMovimentacaoRepository movimentacaoRepository,
        IParticipanteRepository participanteRepository,
        IMapper mapper)
    {
        _movimentacaoRepository = movimentacaoRepository;
        _participanteRepository  = participanteRepository;
        _mapper                  = mapper;
    }

    public async Task<MovimentacaoResponse> Handle(RejeitarMovimentacaoCommand cmd, CancellationToken ct)
    {
        var participante = await _participanteRepository.GetByCofreIdAndUsuarioIdAsync(cmd.CofreId, cmd.UsuarioId, ct);
        var isAdmin = participante?.Role == RoleParticipante.Admin;

        if (!isAdmin)
            throw new DomainValidationException(["Apenas administradores do cofre podem rejeitar movimentações."]);

        var mov = await _movimentacaoRepository.GetByIdAsync(cmd.MovimentacaoId, ct)
            ?? throw new DomainValidationException(["Movimentação não encontrada."]);

        if (mov.CofreId != cmd.CofreId)
            throw new DomainValidationException(["Movimentação não pertence a este cofre."]);

        mov.Rejeitar();
        await _movimentacaoRepository.UpdateAsync(mov, ct);

        return _mapper.Map<MovimentacaoResponse>(mov);
    }
}
