using AutoMapper;
using Ledger.Domain.Entities;
using Ledger.Domain.Interfaces;
using Ledger.Infrastructure.Data;
using Ledger.Infrastructure.Data.Models;

namespace Ledger.Infrastructure.Repositories;

public class ArquivoRepository : BaseRepository<ArquivoDomain, ArquivoModel>, IArquivoRepository
{
    public ArquivoRepository(LedgerDbContext context, IMapper mapper) : base(context, mapper) { }
}