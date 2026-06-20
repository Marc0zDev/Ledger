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
    private readonly ICofreRepository         _cofreRepository;
    private readonly IMovimentacaoRepository  _movimentacaoRepository;
    private readonly IUsuarioRepository       _usuarioRepository;
    private readonly IParticipanteRepository  _participanteRepository;
    private readonly IMapper                  _mapper;

    public RegistrarMovimentacaoCommandHandler(
        ICofreRepository cofreRepository,
        IMovimentacaoRepository movimentacaoRepository,
        IUsuarioRepository usuarioRepository,
        IParticipanteRepository participanteRepository,
        IMapper mapper)
    {
        _cofreRepository        = cofreRepository;
        _movimentacaoRepository = movimentacaoRepository;
        _usuarioRepository      = usuarioRepository;
        _participanteRepository = participanteRepository;
        _mapper                 = mapper;
    }

    public async Task<MovimentacaoResponse> Handle(RegistrarMovimentacaoCommand cmd, CancellationToken ct)
    {
        var cofre = await _cofreRepository.GetByIdAsync(cmd.CofreId, ct)
            ?? throw new DomainValidationException(["Cofre não encontrado."]);

        if (!Enum.TryParse<TipoMovimentacao>(cmd.Tipo, true, out var tipo))
            throw new DomainValidationException([$"Tipo de movimentação inválido: {cmd.Tipo}."]);

        var participante = await _participanteRepository.GetByCofreIdAndUsuarioIdAsync(cmd.CofreId, cmd.UsuarioId, ct);
        var isAdmin = participante?.Role == RoleParticipante.Admin
                   || participante == null; // criador sem registro explícito é tratado como admin

        // Saída por não-admin entra como pendente de aprovação
        var status = (tipo == TipoMovimentacao.Saida && !isAdmin)
            ? StatusMovimentacao.PendenteAprovacao
            : StatusMovimentacao.Aprovada;

        var dataUtc = cmd.Data.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(cmd.Data, DateTimeKind.Utc)
            : cmd.Data.ToUniversalTime();

        var mov = MovimentacaoDomain.Criar(cmd.Descricao, cmd.Valor, tipo, dataUtc, cmd.CofreId, cmd.UsuarioId, status);

        cofre.RegistrarMovimentacao(mov);

        if (!cofre.IsValid)
            throw new DomainValidationException(cofre.Notifications.Select(n => n.Message));

        await _movimentacaoRepository.AddAsync(mov, ct);

        var response = _mapper.Map<MovimentacaoResponse>(mov);
        var usuario  = await _usuarioRepository.GetByIdAsync(cmd.UsuarioId, ct);
        response.UsuarioNome = usuario?.Nome;
        return response;
    }
}
