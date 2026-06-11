using AutoMapper;
using Ledger.Application.DTOs.DespesaPeriodo;
using Ledger.Domain.Entities;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Commands.DespesaPeriodo;

// ── Criar lançamento ──────────────────────────────────────────────────────────
public record CriarDespesaPeriodoCommand(
    Guid?    DespesaId,
    Guid     CategoriaId,
    Guid     UsuarioId,
    string   Descricao,
    decimal  ValorPlanejado,
    DateTime Competencia) : IRequest<DespesaPeriodoResponse>;

public class CriarDespesaPeriodoCommandHandler : IRequestHandler<CriarDespesaPeriodoCommand, DespesaPeriodoResponse>
{
    private readonly IDespesaPeriodoRepository _repo;
    private readonly ICategoriaRepository      _categoriaRepo;
    private readonly IMapper                   _mapper;

    public CriarDespesaPeriodoCommandHandler(IDespesaPeriodoRepository repo,
        ICategoriaRepository categoriaRepo, IMapper mapper)
    {
        _repo          = repo;
        _categoriaRepo = categoriaRepo;
        _mapper        = mapper;
    }

    public async Task<DespesaPeriodoResponse> Handle(CriarDespesaPeriodoCommand cmd, CancellationToken ct)
    {
        var categoria = await _categoriaRepo.GetByIdAsync(cmd.CategoriaId, ct)
            ?? throw new DomainValidationException(["Categoria não encontrada."]);

        var competencia = new DateTime(cmd.Competencia.Year, cmd.Competencia.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var lancamento = DespesaPeriodoDomain.Criar(
            cmd.DespesaId, cmd.CategoriaId, cmd.UsuarioId,
            cmd.Descricao, cmd.ValorPlanejado, competencia);

        if (!lancamento.IsValid)
            throw new DomainValidationException(lancamento.Notifications.Select(n => n.Message));

        await _repo.AddAsync(lancamento, ct);

        var response = _mapper.Map<DespesaPeriodoResponse>(lancamento);
        response.CategoriaNome  = categoria.Nome;
        response.CategoriaIcone = categoria.Icone;
        response.CategoriaCor   = categoria.Cor;
        return response;
    }
}

// ── Atualizar lançamento ──────────────────────────────────────────────────────
public record AtualizarDespesaPeriodoCommand(
    Guid    Id,
    string  Descricao,
    decimal ValorPlanejado,
    Guid    CategoriaId) : IRequest<DespesaPeriodoResponse?>;

public class AtualizarDespesaPeriodoCommandHandler : IRequestHandler<AtualizarDespesaPeriodoCommand, DespesaPeriodoResponse?>
{
    private readonly IDespesaPeriodoRepository _repo;
    private readonly ICategoriaRepository      _categoriaRepo;
    private readonly IMapper                   _mapper;

    public AtualizarDespesaPeriodoCommandHandler(IDespesaPeriodoRepository repo,
        ICategoriaRepository categoriaRepo, IMapper mapper)
    {
        _repo          = repo;
        _categoriaRepo = categoriaRepo;
        _mapper        = mapper;
    }

    public async Task<DespesaPeriodoResponse?> Handle(AtualizarDespesaPeriodoCommand cmd, CancellationToken ct)
    {
        var lancamento = await _repo.GetByIdAsync(cmd.Id, ct);
        if (lancamento is null) return null;

        var categoria = await _categoriaRepo.GetByIdAsync(cmd.CategoriaId, ct)
            ?? throw new DomainValidationException(["Categoria não encontrada."]);

        lancamento.Atualizar(cmd.Descricao, cmd.ValorPlanejado, cmd.CategoriaId);

        if (!lancamento.IsValid)
            throw new DomainValidationException(lancamento.Notifications.Select(n => n.Message));

        await _repo.UpdateAsync(lancamento, ct);

        var response = _mapper.Map<DespesaPeriodoResponse>(lancamento);
        response.CategoriaNome  = categoria.Nome;
        response.CategoriaIcone = categoria.Icone;
        response.CategoriaCor   = categoria.Cor;
        return response;
    }
}

// ── Pagar lançamento ──────────────────────────────────────────────────────────
public record PagarDespesaPeriodoCommand(
    Guid      Id,
    DateTime? DataPagamento  = null,
    decimal?  ValorRealizado = null) : IRequest<DespesaPeriodoResponse?>;

public class PagarDespesaPeriodoCommandHandler : IRequestHandler<PagarDespesaPeriodoCommand, DespesaPeriodoResponse?>
{
    private readonly IDespesaPeriodoRepository _repo;
    private readonly ICategoriaRepository      _categoriaRepo;
    private readonly IMapper                   _mapper;

    public PagarDespesaPeriodoCommandHandler(IDespesaPeriodoRepository repo,
        ICategoriaRepository categoriaRepo, IMapper mapper)
    {
        _repo          = repo;
        _categoriaRepo = categoriaRepo;
        _mapper        = mapper;
    }

    public async Task<DespesaPeriodoResponse?> Handle(PagarDespesaPeriodoCommand cmd, CancellationToken ct)
    {
        var lancamento = await _repo.GetByIdAsync(cmd.Id, ct);
        if (lancamento is null) return null;

        lancamento.Pagar(cmd.DataPagamento, cmd.ValorRealizado);
        await _repo.UpdateAsync(lancamento, ct);

        var categoria = await _categoriaRepo.GetByIdAsync(lancamento.CategoriaId, ct);
        var response  = _mapper.Map<DespesaPeriodoResponse>(lancamento);
        response.CategoriaNome  = categoria?.Nome  ?? string.Empty;
        response.CategoriaIcone = categoria?.Icone;
        response.CategoriaCor   = categoria?.Cor;
        return response;
    }
}

// ── Remover lançamento ────────────────────────────────────────────────────────
public record RemoverDespesaPeriodoCommand(Guid Id) : IRequest<bool>;

public class RemoverDespesaPeriodoCommandHandler : IRequestHandler<RemoverDespesaPeriodoCommand, bool>
{
    private readonly IDespesaPeriodoRepository _repo;
    public RemoverDespesaPeriodoCommandHandler(IDespesaPeriodoRepository repo) => _repo = repo;

    public async Task<bool> Handle(RemoverDespesaPeriodoCommand cmd, CancellationToken ct)
    {
        var item = await _repo.GetByIdAsync(cmd.Id, ct);
        if (item is null) return false;
        await _repo.DeleteAsync(cmd.Id, ct);
        return true;
    }
}

// ── Anexar boleto ─────────────────────────────────────────────────────────────
public record AnexarBoletoPeriodoCommand(Guid Id, string Path) : IRequest<DespesaPeriodoResponse?>;

public class AnexarBoletoPeriodoCommandHandler : IRequestHandler<AnexarBoletoPeriodoCommand, DespesaPeriodoResponse?>
{
    private readonly IDespesaPeriodoRepository _repo;
    private readonly ICategoriaRepository      _categoriaRepo;
    private readonly IMapper                   _mapper;

    public AnexarBoletoPeriodoCommandHandler(IDespesaPeriodoRepository repo,
        ICategoriaRepository categoriaRepo, IMapper mapper)
    {
        _repo          = repo;
        _categoriaRepo = categoriaRepo;
        _mapper        = mapper;
    }

    public async Task<DespesaPeriodoResponse?> Handle(AnexarBoletoPeriodoCommand cmd, CancellationToken ct)
    {
        var lancamento = await _repo.GetByIdAsync(cmd.Id, ct);
        if (lancamento is null) return null;
        lancamento.AnexarBoleto(cmd.Path);
        await _repo.UpdateAsync(lancamento, ct);

        var categoria = await _categoriaRepo.GetByIdAsync(lancamento.CategoriaId, ct);
        var response  = _mapper.Map<DespesaPeriodoResponse>(lancamento);
        response.CategoriaNome  = categoria?.Nome  ?? string.Empty;
        response.CategoriaIcone = categoria?.Icone;
        response.CategoriaCor   = categoria?.Cor;
        return response;
    }
}

// ── Gerar lançamentos do mês a partir dos templates ativos ────────────────────
public record GerarPeriodoCommand(Guid UsuarioId, DateTime Competencia) : IRequest<IEnumerable<DespesaPeriodoResponse>>;

public class GerarPeriodoCommandHandler : IRequestHandler<GerarPeriodoCommand, IEnumerable<DespesaPeriodoResponse>>
{
    private readonly IDespesaRepository        _despesaRepo;
    private readonly IDespesaPeriodoRepository _periodoRepo;
    private readonly ICategoriaRepository      _categoriaRepo;
    private readonly IMapper                   _mapper;

    public GerarPeriodoCommandHandler(IDespesaRepository despesaRepo,
        IDespesaPeriodoRepository periodoRepo, ICategoriaRepository categoriaRepo, IMapper mapper)
    {
        _despesaRepo   = despesaRepo;
        _periodoRepo   = periodoRepo;
        _categoriaRepo = categoriaRepo;
        _mapper        = mapper;
    }

    public async Task<IEnumerable<DespesaPeriodoResponse>> Handle(GerarPeriodoCommand cmd, CancellationToken ct)
    {
        var competencia = new DateTime(cmd.Competencia.Year, cmd.Competencia.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var templates   = await _despesaRepo.GetAtivosAsync(cmd.UsuarioId, ct);
        var categorias  = (await _categoriaRepo.GetByUsuarioIdAsync(cmd.UsuarioId, ct))
                           .ToDictionary(c => c.Id);

        var criados = new List<DespesaPeriodoDomain>();

        foreach (var t in templates)
        {
            // Pular se já existe lançamento gerado para este template neste mês
            if (await _periodoRepo.ExisteParaTemplateNoMesAsync(t.Id, competencia, ct))
                continue;

            var lancamento = DespesaPeriodoDomain.Criar(
                t.Id, t.CategoriaId, t.UsuarioId, t.Nome, t.ValorPlanejado, competencia);

            await _periodoRepo.AddAsync(lancamento, ct);
            criados.Add(lancamento);
        }

        return criados.Select(l =>
        {
            var response = _mapper.Map<DespesaPeriodoResponse>(l);
            if (categorias.TryGetValue(l.CategoriaId, out var cat))
            {
                response.CategoriaNome  = cat.Nome;
                response.CategoriaIcone = cat.Icone;
                response.CategoriaCor   = cat.Cor;
            }
            return response;
        });
    }
}
