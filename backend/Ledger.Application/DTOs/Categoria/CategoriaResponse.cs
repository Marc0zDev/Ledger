namespace Ledger.Application.DTOs.Categoria;

public class CategoriaResponse
{
    public Guid    Id       { get; set; }
    public string  Nome     { get; set; } = string.Empty;
    public string? Icone    { get; set; }
    public string? Cor      { get; set; }
    public bool    IsSystem { get; set; }
}
