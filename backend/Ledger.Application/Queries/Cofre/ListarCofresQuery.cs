using Ledger.Application.DTOs.Cofre;
using MediatR;

namespace Ledger.Application.Queries.Cofre;

/// <summary>Lista os cofres acessíveis ao usuário (criados por ele ou onde é participante).</summary>
public record ListarCofresQuery(Guid UsuarioId) : IRequest<IEnumerable<CofreResponse>>;
