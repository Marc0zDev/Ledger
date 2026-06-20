using AutoMapper;
using Ledger.Application.DTOs.DespesaPeriodo;
using Ledger.Domain.Entities;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Commands.DespesaPeriodo;

// ── Command ───────────────────────────────────────────────────────────────────
public record AnexarComprovantePeriodoCommand(
    Guid   Id,
    string NomeArquivo,
    string ContentType,
    string Extensao,
    byte[] Bytes) : IRequest<DespesaPeriodoResponse?>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class AnexarComprovantePeriodoCommandHandler
    : IRequestHandler<AnexarComprovantePeriodoCommand, DespesaPeriodoResponse?>
{
    private readonly IDespesaPeriodoRepository _repo;
    private readonly IArquivoRepository        _arquivoRepo;
    private readonly ICategoriaRepository      _categoriaRepo;
    private readonly IMapper                   _mapper;

    public AnexarComprovantePeriodoCommandHandler(IDespesaPeriodoRepository repo,
        IArquivoRepository arquivoRepo, ICategoriaRepository categoriaRepo, IMapper mapper)
    {
        _repo          = repo;
        _arquivoRepo   = arquivoRepo;
        _categoriaRepo = categoriaRepo;
        _mapper        = mapper;
    }

    public async Task<DespesaPeriodoResponse?> Handle(AnexarComprovantePeriodoCommand cmd, CancellationToken ct)
    {
        var lancamento = await _repo.GetByIdAsync(cmd.Id, ct);
        if (lancamento is null) return null;

        var arquivo = ArquivoDomain.Criar(cmd.NomeArquivo, cmd.Extensao, cmd.ContentType, cmd.Bytes);
        if (!arquivo.IsValid)
            throw new DomainValidationException(arquivo.Notifications.Select(n => n.Message));

        await _arquivoRepo.AddAsync(arquivo, ct);
        lancamento.AnexarComprovante(arquivo.Id);
        await _repo.UpdateAsync(lancamento, ct);

        var categoria = await _categoriaRepo.GetByIdAsync(lancamento.CategoriaId, ct);
        var response  = _mapper.Map<DespesaPeriodoResponse>(lancamento);
        response.CategoriaNome  = categoria?.Nome  ?? string.Empty;
        response.CategoriaIcone = categoria?.Icone;
        response.CategoriaCor   = categoria?.Cor;
        return response;
    }
}
