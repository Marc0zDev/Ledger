namespace Ledger.Application.DTOs.Receita;

public class ReceitaRequest
{
    public Guid     UsuarioId        { get; set; }
    public string   Nome             { get; set; } = string.Empty;
    public decimal  Valor            { get; set; }
    public string?  Descricao        { get; set; }
    public Guid?    ArquivoId        { get; set; }
    public DateTime DataRecebimento  { get; set; }
    public Guid?    ReceitaTemplateId { get; set; }
}

public class CriarReceitaTemplateRequest
{
    public Guid      UsuarioId  { get; set; }
    public string    Nome       { get; set; } = string.Empty;
    public decimal   Valor      { get; set; }
    public string?   Descricao  { get; set; }
    public DateTime  DataInicio { get; set; }
    public DateTime? DataFim    { get; set; }
}

public class AtualizarReceitaTemplateRequest
{
    public string    Nome       { get; set; } = string.Empty;
    public decimal   Valor      { get; set; }
    public string?   Descricao  { get; set; }
    public DateTime  DataInicio { get; set; }
    public DateTime? DataFim    { get; set; }
}

public class GerarReceitasMesRequest
{
    public Guid     UsuarioId  { get; set; }
    public DateTime Competencia { get; set; }
}
