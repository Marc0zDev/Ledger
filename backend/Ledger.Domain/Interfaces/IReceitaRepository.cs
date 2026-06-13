using Ledger.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ledger.Domain.Interfaces
{
    public interface IReceitaRepository : IRepository<ReceitaDomain>
    {
         ReceitaDomain? GetComDetalhes(Guid id);
         IEnumerable<ReceitaDomain> GetByUsuarioId(Guid usuarioId);
    }
}
