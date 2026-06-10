using Ledger.Application.DTOs.Movimentacao;
using Ledger.Application.DTOs.Participante;

namespace Ledger.Application.DTOs.Cofre;

public class CofreResponse
{
    public Guid     Id               { get; set; }
    public string   Nome             { get; set; } = string.Empty;
    public string?  Descricao        { get; set; }
    public decimal  Meta             { get; set; }
    public string   Status           { get; set; } = string.Empty;
    public string   Categoria        { get; set; } = string.Empty;
    public decimal  TotalMovimentado { get; set; }
    public DateTime  CreatedAt       { get; set; }
    public DateTime? UpdatedAt       { get; set; }
    public IEnumerable<ParticipanteResponse>  Participantes  { get; set; } = [];
    public IEnumerable<MovimentacaoResponse>  Movimentacoes  { get; set; } = [];
}
