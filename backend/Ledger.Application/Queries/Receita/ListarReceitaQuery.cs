using Ledger.Application.DTOs.Receita;
using Ledger.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ledger.Application.Queries.Receita
{
    public record ListarReceitaQuery(
            Guid UsuarioId
        ) : IRequest<IEnumerable<ReceitaResponse>>;

    public class ListarReceitaQueryHandler : IRequestHandler<ListarReceitaQuery, IEnumerable<ReceitaResponse>>
    {
        private readonly IReceitaRepository _receitaRepository;
        public ListarReceitaQueryHandler(IReceitaRepository receitaRepository)
        {
            _receitaRepository = receitaRepository;
        }
        public async Task<IEnumerable<ReceitaResponse>> Handle(ListarReceitaQuery request, CancellationToken cancellationToken)
        {
            var receitas = _receitaRepository.GetByUsuarioId(request.UsuarioId);
            return receitas.Select(r => new ReceitaResponse
            {
                Id = r.Id,
                Nome = r.Nome,
                Valor = r.Valor,
                Descricao = r.Descricao,
                ArquivoId = r.ArquivoId,
                DataRecebimento = r.DataRecebimento,
                DataCriacao = r.CreatedAt
            });
        }
    }
}
