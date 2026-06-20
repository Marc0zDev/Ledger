using Ledger.Application.Interfaces;
using PuppeteerSharp;
using PuppeteerSharp.Media;

namespace Ledger.Infrastructure.Services;

public sealed class BrowserService : IBrowserService, IAsyncDisposable
{
    private IBrowser?            _browser;
    private readonly SemaphoreSlim _lock = new(1, 1);

    private async Task<IBrowser> GetBrowserAsync(CancellationToken ct)
    {
        if (_browser is not null) return _browser;

        await _lock.WaitAsync(ct);
        try
        {
            if (_browser is not null) return _browser;

            var fetcher = new BrowserFetcher();
            await fetcher.DownloadAsync();

            _browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                Args     = ["--no-sandbox", "--disable-setuid-sandbox", "--disable-dev-shm-usage"]
            });
        }
        finally
        {
            _lock.Release();
        }

        return _browser;
    }

    public async Task<byte[]> RenderHtmlToPdfAsync(string html, CancellationToken ct = default)
    {
        var browser = await GetBrowserAsync(ct);

        await using var page = await browser.NewPageAsync();
        await page.SetContentAsync(html, new NavigationOptions { WaitUntil = [WaitUntilNavigation.Networkidle0] });

        return await page.PdfDataAsync(new PdfOptions
        {
            Format          = PaperFormat.A4,
            PrintBackground = true,
            MarginOptions   = new MarginOptions { Top = "20mm", Bottom = "20mm", Left = "15mm", Right = "15mm" }
        });
    }

    public async ValueTask DisposeAsync()
    {
        if (_browser is not null)
            await _browser.CloseAsync();
    }
}
