using Ledger.Application.DTOs.Arquivo;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Queries.Arquivo;

public record ObterArquivoQuery(Guid ArquivoId, Guid UsuarioId) : IRequest<ArquivoConteudoResponse?>;

public class ObterArquivoQueryHandler : IRequestHandler<ObterArquivoQuery, ArquivoConteudoResponse?>
{
    private readonly IArquivoRepository _arquivoRepo;
    private readonly IDespesaRepository _despesaRepo;

    public ObterArquivoQueryHandler(IArquivoRepository arquivoRepo, IDespesaRepository despesaRepo)
    {
        _arquivoRepo = arquivoRepo;
        _despesaRepo = despesaRepo;
    }

    public async Task<ArquivoConteudoResponse?> Handle(ObterArquivoQuery query, CancellationToken ct)
    {
        if (!await _despesaRepo.UsuarioPossuiArquivoAsync(query.ArquivoId, query.UsuarioId, ct))
            return null;

        var arquivo = await _arquivoRepo.GetByIdAsync(query.ArquivoId, ct);
        if (arquivo is null) return null;

        return new ArquivoConteudoResponse
        {
            Nome        = arquivo.Nome,
            ContentType = arquivo.ContentType,
            ArquivoByte = arquivo.ArquivoByte,
        };
    }
}
