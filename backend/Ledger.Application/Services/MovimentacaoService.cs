using AutoMapper;
using Ledger.Application.DTOs.Movimentacao;
using Ledger.Application.Interfaces;
using Ledger.Domain.Entities;
using Ledger.Domain.Enums;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Interfaces;

namespace Ledger.Application.Services;

public class MovimentacaoService : IMovimentacaoService
{
    private readonly IMovimentacaoRepository _movimentacaoRepository;
    private readonly ICofreRepository        _cofreRepository;
    private readonly IMapper                 _mapper;

    public MovimentacaoService(
        IMovimentacaoRepository movimentacaoRepository,
        ICofreRepository cofreRepository,
        IMapper mapper)
    {
        _movimentacaoRepository = movimentacaoRepository;
        _cofreRepository        = cofreRepository;
        _mapper                 = mapper;
    }

    public async Task<IEnumerable<MovimentacaoResponse>> ListarPorCofreAsync(Guid cofreId, CancellationToken ct = default)
    {
        var movs = await _movimentacaoRepository.GetByCofreIdAsync(cofreId, ct);
        return _mapper.Map<IEnumerable<MovimentacaoResponse>>(movs);
    }

    public async Task<MovimentacaoResponse> RegistrarAsync(Guid cofreId, Guid usuarioId, CriarMovimentacaoRequest request, CancellationToken ct = default)
    {
        var cofre = await _cofreRepository.GetByIdAsync(cofreId, ct)
            ?? throw new DomainValidationException(["Cofre não encontrado."]);

        if (!Enum.TryParse<TipoMovimentacao>(request.Tipo, ignoreCase: true, out var tipo))
            throw new DomainValidationException([$"Tipo de movimentação inválido: '{request.Tipo}'. Use 'Entrada' ou 'Saida'."]);

        var movimentacao = MovimentacaoDomain.Criar(request.Descricao, request.Valor, tipo, request.Data, cofreId, usuarioId);
        cofre.RegistrarMovimentacao(movimentacao);

        if (!cofre.IsValid)
            throw new DomainValidationException(cofre.Notifications.Select(n => n.Message));

        await _movimentacaoRepository.AddAsync(movimentacao, ct);
        return _mapper.Map<MovimentacaoResponse>(movimentacao);
    }
}
