namespace Ledger.Infrastructure.Data.Models;

public class ReceitaModel
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public string? Descricao { get; set; }
    public Guid? ArquivoId { get; set; }
    public DateTime DataRecebimento { get; set; }
    public DateTime Competencia { get; set; }
    public Guid? ReceitaTemplateId { get; set; }
    public Guid UsuarioId { get; set; }
    public Guid? GrupoId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public virtual ArquivoModel? Arquivo { get; set; }
    public virtual ApplicationUser Usuario { get; set; } = null!;
    public virtual ReceitaTemplateModel? Template { get; set; }
    public virtual GrupoModel? Grupo { get; set; }
}
