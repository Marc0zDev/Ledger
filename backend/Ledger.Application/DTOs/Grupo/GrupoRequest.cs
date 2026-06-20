namespace Ledger.Application.DTOs.Grupo;

public record CriarGrupoRequest(string Nome, string? Descricao);

public record AtualizarGrupoRequest(string Nome, string? Descricao);

public record AdicionarMembroGrupoRequest(Guid UsuarioId);

public record AlterarRoleMembroGrupoRequest(string Role);
