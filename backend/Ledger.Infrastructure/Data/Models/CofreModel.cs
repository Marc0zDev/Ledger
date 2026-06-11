namespace Ledger.Infrastructure.Data.Models;

public class CofreModel
{
    public Guid     Id                 { get; set; }
    public string   Nome               { get; set; } = string.Empty;
    public string?  Descricao          { get; set; }
    public decimal  Meta               { get; set; }
    public int      Status             { get; set; }
    public int      Categoria          { get; set; }
    public int      Visibilidade       { get; set; } = 1; // Privado
    public Guid     CriadoPorUsuarioId { get; set; }
    public DateTime  CreatedAt         { get; set; }
    public DateTime? UpdatedAt         { get; set; }

    public List<ParticipanteModel>  Participantes  { get; set; } = new();
    public List<MovimentacaoModel>  Movimentacoes  { get; set; } = new();
}
