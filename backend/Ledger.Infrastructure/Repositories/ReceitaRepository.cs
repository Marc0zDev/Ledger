using AutoMapper;
using Ledger.Domain.Entities;
using Ledger.Domain.Interfaces;
using Ledger.Infrastructure.Data;
using Ledger.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ledger.Infrastructure.Repositories
{
    public class ReceitaRepository : BaseRepository<ReceitaDomain, ReceitaModel>, IReceitaRepository
    {

        public ReceitaRepository(LedgerDbContext context, IMapper mapper) : base(context, mapper) { }


        public IEnumerable<ReceitaDomain> GetByUsuarioId(Guid usuarioId)
        {
            var models = DbSet
                .AsNoTracking()
                .Where(r => r.UsuarioId == usuarioId)
                .ToList();

            return Mapper.Map<IEnumerable<ReceitaDomain>>(models);
        }

        public ReceitaDomain? GetComDetalhes(Guid id)
        {
            var model = DbSet
                .Include(r => r.Arquivo)
                .Include(a => a.Usuario)
                .FirstOrDefault(r => r.Id == id);

            return model is null ? null : Mapper.Map<ReceitaDomain>(model);
        }
    }
}
