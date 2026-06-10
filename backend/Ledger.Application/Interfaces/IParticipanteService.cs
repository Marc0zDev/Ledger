using Ledger.Application.DTOs.Participante;

namespace Ledger.Application.Interfaces;

public interface IParticipanteService
{
    Task<IEnumerable<ParticipanteResponse>> ListarPorCofreAsync(Guid cofreId, CancellationToken ct = default);
    Task<ParticipanteResponse> AdicionarAsync(Guid cofreId, CriarParticipanteRequest request, CancellationToken ct = default);
    Task<bool> RemoverAsync(Guid cofreId, Guid participanteId, CancellationToken ct = default);
}
