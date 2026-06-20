namespace Ledger.Application.DTOs.Arquivo;

public class ArquivoConteudoResponse
{
    public string Nome        { get; set; } = string.Empty;
    public string ContentType { get; set; } = "application/octet-stream";
    public byte[] ArquivoByte { get; set; } = Array.Empty<byte>();
}
