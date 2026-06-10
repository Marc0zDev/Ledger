using Ledger.Application.DTOs.Auth;
using Ledger.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Ledger.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService     _authService;
    private readonly IIdentityService _identityService;

    public AuthController(IAuthService authService, IIdentityService identityService)
    {
        _authService     = authService;
        _identityService = identityService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await _authService.LoginAsync(request, ct);
        return Ok(result);
    }

    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar([FromBody] RegistrarRequest request, CancellationToken ct)
    {
        var result = await _authService.RegistrarAsync(request, ct);
        return Created(string.Empty, result);
    }

    /// <summary>Verifica se o e-mail de um usuário já foi confirmado (usado para polling).</summary>
    [HttpGet("email-confirmado")]
    public async Task<IActionResult> EmailConfirmado([FromQuery] Guid userId, CancellationToken ct)
    {
        var confirmado = await _identityService.EmailConfirmadoAsync(userId, ct);
        return Ok(new { confirmado });
    }

    /// <summary>Confirma o e-mail do usuário a partir do token enviado por e-mail.</summary>
    [HttpGet("confirmar-email")]
    public async Task<ContentResult> ConfirmarEmail(
        [FromQuery] Guid userId,
        [FromQuery] string token,
        CancellationToken ct)
    {
        var sucesso = await _identityService.ConfirmarEmailAsync(userId, token, ct);

        var frontendUrl = "http://localhost:4200/login";
        var html = sucesso
            ? ConfirmacaoHtml(
                titulo:  "E-mail confirmado!",
                icone:   "✓",
                corIcone: "#2ECC71",
                mensagem: "Sua conta foi ativada com sucesso. Você já pode fazer login no Ledger.",
                linkTexto: "Ir para o login",
                linkUrl:   $"{frontendUrl}?confirmed=true")
            : ConfirmacaoHtml(
                titulo:  "Link inválido ou expirado",
                icone:   "✕",
                corIcone: "#E74C3C",
                mensagem: "Este link de confirmação não é mais válido. Faça login e solicite um novo e-mail de confirmação.",
                linkTexto: "Ir para o login",
                linkUrl:   frontendUrl);

        return new ContentResult
        {
            Content     = html,
            ContentType = "text/html; charset=utf-8",
            StatusCode  = sucesso ? 200 : 400,
        };
    }

    private static string ConfirmacaoHtml(string titulo, string icone, string corIcone, string mensagem, string linkTexto, string linkUrl) => $$"""
        <!DOCTYPE html>
        <html lang="pt-BR">
        <head>
          <meta charset="UTF-8">
          <meta name="viewport" content="width=device-width,initial-scale=1">
          <title>{{titulo}} — Ledger</title>
          <style>
            * { box-sizing: border-box; margin: 0; padding: 0; }
            body { background: #F5F3EF; font-family: 'Segoe UI', Arial, sans-serif; min-height: 100dvh; display: flex; align-items: center; justify-content: center; padding: 24px; }
            .card { background: #fff; border: 1px solid #E0DDD8; border-radius: 20px; padding: 48px 40px; max-width: 420px; width: 100%; text-align: center; }
            .brand { display: flex; align-items: center; justify-content: center; gap: 10px; margin-bottom: 36px; }
            .brand-letter { width: 40px; height: 40px; background: #1A1714; border-radius: 10px; display: flex; align-items: center; justify-content: center; font-family: Georgia, serif; font-size: 20px; font-weight: bold; color: #fff; }
            .brand-name { font-family: Georgia, serif; font-size: 22px; color: #1A1714; font-weight: 600; }
            .icon { width: 72px; height: 72px; border-radius: 50%; background: {{corIcone}}18; border: 2px solid {{corIcone}}; display: flex; align-items: center; justify-content: center; margin: 0 auto 24px; font-size: 28px; color: {{corIcone}}; }
            h1 { font-family: Georgia, serif; font-size: 24px; font-weight: 400; color: #1A1714; margin-bottom: 12px; }
            p { font-size: 15px; color: #6B6460; line-height: 1.6; margin-bottom: 32px; }
            a.btn { display: inline-block; background: #1A1714; color: #fff; text-decoration: none; padding: 13px 32px; border-radius: 10px; font-size: 15px; font-weight: 600; }
            a.btn:hover { opacity: 0.85; }
          </style>
        </head>
        <body>
          <div class="card">
            <div class="brand">
              <div class="brand-letter">L</div>
              <span class="brand-name">Ledger</span>
            </div>
            <div class="icon">{{icone}}</div>
            <h1>{{titulo}}</h1>
            <p>{{mensagem}}</p>
            <a class="btn" href="{{linkUrl}}">{{linkTexto}}</a>
          </div>
        </body>
        </html>
        """;
}
