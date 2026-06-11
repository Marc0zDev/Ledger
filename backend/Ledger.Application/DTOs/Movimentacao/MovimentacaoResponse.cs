namespace Ledger.Application.DTOs.Movimentacao;

public class MovimentacaoResponse
{
    public Guid      Id           { get; set; }
    public string    Descricao    { get; set; } = string.Empty;
    public decimal   Valor        { get; set; }
    public string    Tipo         { get; set; } = string.Empty;
    public DateTime  Data         { get; set; }
    public Guid      CofreId      { get; set; }
    public Guid      UsuarioId    { get; set; }
    public string?   UsuarioNome  { get; set; }
    public DateTime  CreatedAt    { get; set; }
    public DateTime? UpdatedAt    { get; set; }
}
