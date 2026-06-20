using AutoMapper;
using Ledger.Application.DTOs.Arquivo;
using Ledger.Application.DTOs.Cofre;
using Ledger.Application.Events;
using Ledger.Domain.Entities;
using Ledger.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ledger.Application.Commands.Arquivo
{
    public record RegistrarArquivoCommand(Guid DespesaId, string NomeArquivo, string Content,byte[] ArquivoByte, string Extensao) : IRequest<ArquivoResponse>;
    public class RegistrarArquivoCommandHandler : IRequestHandler<RegistrarArquivoCommand, ArquivoResponse>
    {
        private readonly IArquivoRepository _arquivoRepository;
        private readonly IDespesaRepository _despesaRepository;
        private readonly IDomainEventDispatcher _dispatcher;
        private readonly IMapper _mapper;


        public RegistrarArquivoCommandHandler(IArquivoRepository arquivorepository, IDespesaRepository despesaRepository, IDomainEventDispatcher dispatcher, IMapper mapper)
        {
            _arquivoRepository = arquivorepository;
            _despesaRepository = despesaRepository;
            _dispatcher = dispatcher;
            _mapper = mapper;
        }
        public async Task<ArquivoResponse> Handle(RegistrarArquivoCommand cmd, CancellationToken ct)
        {
            var arquivo = ArquivoDomain.Criar(cmd.NomeArquivo, cmd.Extensao,  cmd.Content ,cmd.ArquivoByte);
            if(!arquivo.IsValid)
                throw new Exception(string.Join(", ", arquivo.Notifications.Select(n => n.Message)));

            await _arquivoRepository.AddAsync(arquivo, ct);

            var despesa = await _despesaRepository.GetByIdAsync(cmd.DespesaId, ct);
            if (despesa == null)
                throw new Exception("Despesa não encontrada.");

            despesa.AdicionarArquivo(arquivo.Id);
            await _despesaRepository.UpdateAsync(despesa, ct);

            await _dispatcher.DispatchAsync(arquivo, ct);

            return _mapper.Map<ArquivoResponse>(arquivo);
        }
    }
}
