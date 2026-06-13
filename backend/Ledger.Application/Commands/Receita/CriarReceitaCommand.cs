using Ledger.Domain.Entities;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ledger.Application.Commands.Receita
{
    public record CriarReceitaCommand(
         Guid UsuarioId,
            string Nome,
            decimal Valor,
            string Descricao,
            Guid? ArquivoId,
            DateTime DataRecebimento
        ) : IRequest<Guid>;

    public class CriarReceitaCommandHandler : IRequestHandler<CriarReceitaCommand, Guid>
    {
        private readonly IReceitaRepository _receitaRepository;
        public CriarReceitaCommandHandler(IReceitaRepository receitaRepository)
        {
            _receitaRepository = receitaRepository;
        }
        public async Task<Guid> Handle(CriarReceitaCommand request, CancellationToken cancellationToken)
        {
            var dataRecebimento = DateTime.SpecifyKind(request.DataRecebimento, DateTimeKind.Utc);
            var receitaDomain = ReceitaDomain.Criar(request.Nome, request.Valor, request.Descricao, request.ArquivoId, dataRecebimento, request.UsuarioId);
            if(!receitaDomain.IsValid)
                throw new DomainValidationException(receitaDomain.Notifications.Select(n => n.Message));

            await _receitaRepository.AddAsync(receitaDomain);
            return receitaDomain.Id;
        }
    }
}
