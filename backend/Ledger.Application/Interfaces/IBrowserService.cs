namespace Ledger.Application.Interfaces;

/// <summary>
/// Gerencia uma única instância de navegador Chromium (PuppeteerSharp).
/// Registrado como Singleton para evitar múltiplas instâncias por request.
/// </summary>
public interface IBrowserService
{
    Task<byte[]> RenderHtmlToPdfAsync(string html, CancellationToken ct = default);
}
