using Ledger.Application.DTOs.Cofre;
using MediatR;

namespace Ledger.Application.Commands.Cofre;

/// <summary>Comando para concluir um cofre. Despacha via IMediator.Send().</summary>
public record ConcluirCofreCommand(Guid CofreId) : IRequest<CofreResponse?>;
