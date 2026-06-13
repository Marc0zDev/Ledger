using Ledger.Application.Commands.Receita;
using Ledger.Application.DTOs.Receita;
using Ledger.Application.Queries.Receita;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ledger.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceitaController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReceitaController(IMediator mediator) => _mediator = mediator;

        [HttpGet("{idusario}")]
        public async Task<IActionResult> ListarPorUsuario(Guid idusario,CancellationToken ct)
        {
            var receitas = await _mediator.Send(new ListarReceitaQuery(idusario), ct);
            return Ok(receitas);
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] ReceitaRequest request, CancellationToken ct)
        {
            var receitaId = await _mediator.Send(
                new CriarReceitaCommand(
                    request.UsuarioId, 
                    request.Nome,
                    request.Valor, 
                    request.Descricao, 
                    request.ArquivoId, 
                    request.DataRecebimento
                ), 
            ct);

            return CreatedAtAction(nameof(ListarPorUsuario), new { idusario = request.UsuarioId }, new { Id = receitaId });
        }
    }
}
