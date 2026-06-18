namespace Ledger.Application.DTOs.Receita;

public class ReceitaResponse
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public string? Descricao { get; set; }
    public Guid? ArquivoId { get; set; }
    public DateTime DataRecebimento { get; set; }
    public DateTime Competencia { get; set; }
    public Guid? ReceitaTemplateId { get; set; }
    public Guid? GrupoId { get; set; }
    public string? UsuarioNome { get; set; }
    public DateTime CreatedAt { get; set; }
}
