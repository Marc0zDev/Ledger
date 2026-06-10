namespace Ledger.Application.Templates;

public static class EmailTemplates
{
    private const string PrimaryColor = "#1A1714";
    private const string GoldColor    = "#A8742A";
    private const string BgColor      = "#F5F3EF";
    private const string CardBg       = "#FFFFFF";
    private const string MutedColor   = "#6B6460";
    private const string BorderColor  = "#E0DDD8";

    // ── Shared layout helpers ─────────────────────────────────────────────

    private static string Header() => $"""
              <tr>
                <td style="background:{PrimaryColor};padding:32px 40px;">
                  <table cellpadding="0" cellspacing="0">
                    <tr>
                      <td style="background:{GoldColor};width:40px;height:40px;border-radius:10px;text-align:center;vertical-align:middle;">
                        <span style="font-family:Georgia,serif;font-size:22px;font-weight:bold;color:#fff;line-height:40px;display:block;">L</span>
                      </td>
                      <td style="padding-left:12px;">
                        <span style="font-family:Georgia,serif;font-size:22px;color:#fff;font-weight:600;">Ledger</span>
                      </td>
                    </tr>
                  </table>
                </td>
              </tr>
        """;

    private static string Footer() => $"""
              <tr><td style="padding:0 40px;"><hr style="border:none;border-top:1px solid {BorderColor};margin:0;"></td></tr>
              <tr>
                <td style="padding:24px 40px;">
                  <p style="margin:0;font-size:12px;color:{MutedColor};line-height:1.6;">
                    Este é um e-mail automático do Ledger. Por favor, não responda.
                  </p>
                </td>
              </tr>
        """;

    private static string Wrap(string body) => $"""
        <!DOCTYPE html>
        <html lang="pt-BR">
        <head><meta charset="UTF-8"><meta name="viewport" content="width=device-width,initial-scale=1"></head>
        <body style="margin:0;padding:0;background:{BgColor};font-family:'Segoe UI',Arial,sans-serif;">
          <table width="100%" cellpadding="0" cellspacing="0" style="background:{BgColor};padding:40px 16px;">
            <tr><td align="center">
              <table width="520" cellpadding="0" cellspacing="0" style="background:{CardBg};border-radius:16px;border:1px solid {BorderColor};overflow:hidden;">
        {body}
              </table>
            </td></tr>
          </table>
        </body>
        </html>
        """;

    // ── Templates ─────────────────────────────────────────────────────────

    /// <summary>E-mail de confirmação de conta.</summary>
    public static string ConfirmacaoConta(string nome, string linkConfirmacao) => Wrap($"""
        {Header()}
              <tr>
                <td style="padding:40px 40px 32px;">
                  <h1 style="margin:0 0 8px;font-family:Georgia,serif;font-size:26px;font-weight:400;color:{PrimaryColor};">
                    Confirme seu e-mail
                  </h1>
                  <p style="margin:0 0 24px;font-size:15px;color:{MutedColor};line-height:1.6;">
                    Olá, <strong style="color:{PrimaryColor};">{nome}</strong>! Bem-vindo ao Ledger.<br>
                    Clique no botão abaixo para ativar sua conta e começar a organizar suas finanças compartilhadas.
                  </p>
                  <table cellpadding="0" cellspacing="0" style="margin:0 0 8px;">
                    <tr>
                      <td style="background:{PrimaryColor};border-radius:10px;">
                        <a href="{linkConfirmacao}"
                           style="display:inline-block;padding:14px 32px;font-size:15px;font-weight:600;color:#fff;text-decoration:none;letter-spacing:0.02em;">
                          Confirmar e-mail
                        </a>
                      </td>
                    </tr>
                  </table>
                </td>
              </tr>
        {Footer()}
        """);

    /// <summary>Convite para participar de um cofre.</summary>
    public static string ConviteCofre(string nomeConvidado, string nomeConvidante, string nomeCofre, string linkApp) => Wrap($"""
        {Header()}
              <tr>
                <td style="padding:40px 40px 32px;">
                  <h1 style="margin:0 0 8px;font-family:Georgia,serif;font-size:26px;font-weight:400;color:{PrimaryColor};">
                    Você foi convidado!
                  </h1>
                  <p style="margin:0 0 16px;font-size:15px;color:{MutedColor};line-height:1.6;">
                    Olá, <strong style="color:{PrimaryColor};">{nomeConvidado}</strong>!<br>
                    <strong style="color:{PrimaryColor};">{nomeConvidante}</strong> adicionou você ao cofre compartilhado:
                  </p>
                  <div style="margin:0 0 24px;padding:16px 20px;background:{BgColor};border-radius:10px;border-left:4px solid {GoldColor};">
                    <span style="font-family:Georgia,serif;font-size:18px;color:{PrimaryColor};font-weight:600;">{nomeCofre}</span>
                  </div>
                  <table cellpadding="0" cellspacing="0" style="margin:0 0 8px;">
                    <tr>
                      <td style="background:{PrimaryColor};border-radius:10px;">
                        <a href="{linkApp}"
                           style="display:inline-block;padding:14px 32px;font-size:15px;font-weight:600;color:#fff;text-decoration:none;letter-spacing:0.02em;">
                          Ver meus cofres
                        </a>
                      </td>
                    </tr>
                  </table>
                </td>
              </tr>
        {Footer()}
        """);

    /// <summary>Notificação de cofre concluído — enviado a todos os participantes.</summary>
    public static string CofreConcluido(string nomeParticipante, string nomeCofre, decimal meta, decimal totalMovimentado) => Wrap($"""
        {Header()}
              <tr>
                <td style="padding:40px 40px 32px;">
                  <h1 style="margin:0 0 8px;font-family:Georgia,serif;font-size:26px;font-weight:400;color:{PrimaryColor};">
                    Cofre concluído! 🎉
                  </h1>
                  <p style="margin:0 0 16px;font-size:15px;color:{MutedColor};line-height:1.6;">
                    Olá, <strong style="color:{PrimaryColor};">{nomeParticipante}</strong>!<br>
                    O cofre ao qual você participa foi marcado como <strong style="color:{GoldColor};">concluído</strong>.
                  </p>
                  <div style="margin:0 0 24px;padding:20px;background:{BgColor};border-radius:12px;border:1px solid {BorderColor};">
                    <p style="margin:0 0 4px;font-size:13px;color:{MutedColor};text-transform:uppercase;letter-spacing:0.06em;">Cofre</p>
                    <p style="margin:0 0 16px;font-family:Georgia,serif;font-size:20px;color:{PrimaryColor};font-weight:600;">{nomeCofre}</p>
                    <table width="100%" cellpadding="0" cellspacing="0">
                      <tr>
                        <td style="padding:8px 0;border-top:1px solid {BorderColor};">
                          <span style="font-size:13px;color:{MutedColor};">Meta</span>
                          <span style="float:right;font-size:14px;font-weight:600;color:{PrimaryColor};">R$ {meta:N2}</span>
                        </td>
                      </tr>
                      <tr>
                        <td style="padding:8px 0;border-top:1px solid {BorderColor};">
                          <span style="font-size:13px;color:{MutedColor};">Total movimentado</span>
                          <span style="float:right;font-size:14px;font-weight:600;color:{GoldColor};">R$ {totalMovimentado:N2}</span>
                        </td>
                      </tr>
                    </table>
                  </div>
                  <p style="margin:0;font-size:14px;color:{MutedColor};line-height:1.6;">
                    Obrigado por usar o <strong style="color:{PrimaryColor};">Ledger</strong> para organizar suas finanças compartilhadas.
                  </p>
                </td>
              </tr>
        {Footer()}
        """);
}
