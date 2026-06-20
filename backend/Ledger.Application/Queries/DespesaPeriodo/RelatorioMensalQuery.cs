using System.Globalization;
using System.Text;
using Ledger.Application.Interfaces;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Queries.DespesaPeriodo;

public record GerarRelatorioMensalQuery(Guid UsuarioId, DateTime Competencia) : IRequest<byte[]>;

public class GerarRelatorioMensalQueryHandler : IRequestHandler<GerarRelatorioMensalQuery, byte[]>
{
    private readonly IDespesaPeriodoRepository _repo;
    private readonly ICategoriaRepository      _categoriaRepo;
    private readonly IBrowserService           _browser;

    public GerarRelatorioMensalQueryHandler(
        IDespesaPeriodoRepository repo,
        ICategoriaRepository categoriaRepo,
        IBrowserService browser)
    {
        _repo          = repo;
        _categoriaRepo = categoriaRepo;
        _browser       = browser;
    }

    public async Task<byte[]> Handle(GerarRelatorioMensalQuery query, CancellationToken ct)
    {
        var competencia = new DateTime(query.Competencia.Year, query.Competencia.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var lancamentos = (await _repo.GetByCompetenciaAsync(query.UsuarioId, competencia, ct)).ToList();
        var categorias  = (await _categoriaRepo.GetByUsuarioIdAsync(query.UsuarioId, ct))
                           .ToDictionary(c => c.Id);

        var totalPlanejado = lancamentos.Sum(l => l.ValorPlanejado);
        var totalPago      = lancamentos.Where(l => l.Paga).Sum(l => l.ValorRealizado);
        var totalPendente  = lancamentos.Where(l => !l.Paga).Sum(l => l.ValorPlanejado);
        var qtdPagas       = lancamentos.Count(l => l.Paga);

        var porCategoria = lancamentos
            .GroupBy(l => l.CategoriaId)
            .Select(g =>
            {
                categorias.TryGetValue(g.Key, out var cat);
                return new
                {
                    Nome      = cat?.Nome ?? "Sem categoria",
                    Cor       = cat?.Cor  ?? "#888888",
                    Planejado = g.Sum(l => l.ValorPlanejado),
                    Realizado = g.Where(l => l.Paga).Sum(l => l.ValorRealizado),
                    Qtd       = g.Count(),
                    Pagas     = g.Count(l => l.Paga)
                };
            })
            .OrderByDescending(c => c.Planejado)
            .ToList();

        var ptBR    = new CultureInfo("pt-BR");
        var mesNome = competencia.ToString("MMMM yyyy", ptBR);
        mesNome     = char.ToUpper(mesNome[0]) + mesNome[1..];

        var sbCat = new StringBuilder();
        foreach (var c in porCategoria)
        {
            var pct       = totalPlanejado > 0 ? (double)(c.Planejado / totalPlanejado * 100m) : 0;
            var diff      = c.Realizado - c.Planejado;
            var diffClass = diff > 0 ? "over" : diff < 0 ? "under" : "equal";
            var diffStr   = diff == 0 ? "—" : (diff > 0 ? $"+R$ {diff:N2}" : $"R$ {diff:N2}");
            sbCat.Append($"""
                <tr>
                  <td><span class="cat-dot" style="background:{c.Cor}"></span>{c.Nome}</td>
                  <td class="num">R$ {c.Planejado:N2}</td>
                  <td class="num">R$ {c.Realizado:N2}</td>
                  <td class="num {diffClass}">{diffStr}</td>
                  <td class="num">{c.Pagas}/{c.Qtd}</td>
                  <td><div class="bar-bg"><div class="bar-fill" style="width:{Math.Min(pct, 100):F1}%;background:{c.Cor}"></div></div></td>
                </tr>
                """);
        }

        var sbDet = new StringBuilder();
        foreach (var l in lancamentos.OrderBy(l => l.Descricao))
        {
            categorias.TryGetValue(l.CategoriaId, out var cat);
            var badge  = l.Paga
                ? "<span class='badge paga'>Paga</span>"
                : "<span class='badge pendente'>Pendente</span>";
            var pagaEm = l.PagaEm.HasValue ? l.PagaEm.Value.ToString("dd/MM/yyyy") : "—";
            var pago   = l.Paga ? $"R$ {l.ValorRealizado:N2}" : "—";
            sbDet.Append($"""
                <tr>
                  <td>{l.Descricao}</td>
                  <td>{cat?.Nome ?? "—"}</td>
                  <td class="num">R$ {l.ValorPlanejado:N2}</td>
                  <td class="num">{pago}</td>
                  <td>{pagaEm}</td>
                  <td>{badge}</td>
                </tr>
                """);
        }

        var geradoEm = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

        var html = $$"""
            <!DOCTYPE html>
            <html lang="pt-BR">
            <head>
            <meta charset="UTF-8">
            <style>
              @import url('https://fonts.googleapis.com/css2?family=Cormorant+Garamond:wght@400;600&family=DM+Sans:wght@400;500;600&display=swap');
              * { box-sizing: border-box; margin: 0; padding: 0; }
              body { font-family: 'DM Sans', sans-serif; font-size: 12px; color: #1A1714; background: #fff; }
              header { background: #E8E4DE; padding: 28px 40px 20px; border-bottom: 2px solid #C9C5BE; }
              header h1 { font-family: 'Cormorant Garamond', serif; font-size: 28px; font-weight: 400; color: #1A1714; }
              header p { color: #6B6460; font-size: 11px; margin-top: 4px; }
              .summary { display: flex; border-bottom: 1px solid #E0DDD8; }
              .summary-card { flex: 1; padding: 20px 40px; border-right: 1px solid #E0DDD8; }
              .summary-card:last-child { border-right: none; }
              .summary-card .label { font-size: 10px; text-transform: uppercase; letter-spacing: .07em; color: #6B6460; font-weight: 600; }
              .summary-card .value { font-family: 'Cormorant Garamond', serif; font-size: 22px; margin-top: 4px; }
              .value.success { color: #2E7D5A; }
              .value.warn { color: #966A26; }
              section { padding: 24px 40px; }
              section h2 { font-family: 'Cormorant Garamond', serif; font-size: 18px; font-weight: 400; margin-bottom: 14px; border-bottom: 1px solid #E0DDD8; padding-bottom: 6px; }
              table { width: 100%; border-collapse: collapse; font-size: 11.5px; }
              thead tr { background: #E8E4DE; }
              th { padding: 8px 10px; text-align: left; font-size: 10px; font-weight: 700; text-transform: uppercase; letter-spacing: .06em; color: #6B6460; white-space: nowrap; }
              td { padding: 7px 10px; border-bottom: 1px solid #F0EDE8; vertical-align: middle; }
              tr:last-child td { border-bottom: none; }
              .num { text-align: right; font-variant-numeric: tabular-nums; }
              .cat-dot { display: inline-block; width: 8px; height: 8px; border-radius: 50%; margin-right: 6px; vertical-align: middle; }
              .bar-bg { height: 6px; background: #E0DDD8; border-radius: 3px; min-width: 80px; }
              .bar-fill { height: 6px; border-radius: 3px; }
              .over { color: #C0392B; }
              .under { color: #2E7D5A; }
              .badge { display: inline-block; padding: 2px 8px; border-radius: 4px; font-size: 10px; font-weight: 600; }
              .badge.paga { background: #C8DCCC; color: #42874A; }
              .badge.pendente { background: #D4C4AE; color: #966A26; }
              footer { text-align: center; padding: 16px; font-size: 10px; color: #A09B95; border-top: 1px solid #E0DDD8; }
            </style>
            </head>
            <body>
              <header>
                <h1>Relatório Mensal — {{mesNome}}</h1>
                <p>Gerado em {{geradoEm}}</p>
              </header>

              <div class="summary">
                <div class="summary-card">
                  <div class="label">Total Planejado</div>
                  <div class="value">R$ {{totalPlanejado:N2}}</div>
                </div>
                <div class="summary-card">
                  <div class="label">Total Pago</div>
                  <div class="value success">R$ {{totalPago:N2}}</div>
                </div>
                <div class="summary-card">
                  <div class="label">Pendente</div>
                  <div class="value warn">R$ {{totalPendente:N2}}</div>
                </div>
                <div class="summary-card">
                  <div class="label">Lançamentos</div>
                  <div class="value">{{qtdPagas}}/{{lancamentos.Count}} pagos</div>
                </div>
              </div>

              <section>
                <h2>Por Categoria</h2>
                <table>
                  <thead>
                    <tr>
                      <th>Categoria</th>
                      <th class="num">Planejado</th>
                      <th class="num">Realizado</th>
                      <th class="num">Diferença</th>
                      <th class="num">Pagas</th>
                      <th style="min-width:100px">% do Total</th>
                    </tr>
                  </thead>
                  <tbody>{{sbCat}}</tbody>
                </table>
              </section>

              <section>
                <h2>Lançamentos Detalhados</h2>
                <table>
                  <thead>
                    <tr>
                      <th>Descrição</th>
                      <th>Categoria</th>
                      <th class="num">Planejado</th>
                      <th class="num">Pago</th>
                      <th>Data Pgto</th>
                      <th>Status</th>
                    </tr>
                  </thead>
                  <tbody>{{sbDet}}</tbody>
                </table>
              </section>

              <footer>Ledger · Relatório gerado automaticamente</footer>
            </body>
            </html>
            """;

        return await _browser.RenderHtmlToPdfAsync(html, ct);
    }
}
