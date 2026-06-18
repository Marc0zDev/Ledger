using System.Text;
using Ledger.Application.Events;
using Ledger.Application.Interfaces;
using Ledger.Application.Profiles;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Interfaces;
using Ledger.Infrastructure.Data;
using Ledger.Infrastructure.Data.Models;
using Ledger.Infrastructure.Events;
using Ledger.Infrastructure.Profiles;
using Ledger.Infrastructure.Repositories;
using Ledger.Infrastructure.Services;using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Database
builder.Services.AddDbContext<LedgerDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ASP.NET Core Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
    {
        options.Password.RequireDigit           = true;
        options.Password.RequiredLength         = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase       = false;
        options.SignIn.RequireConfirmedEmail     = false; // habilitar em produção
    })
    .AddEntityFrameworkStores<LedgerDbContext>()
    .AddDefaultTokenProviders();

// MediatR — Command/Query/Event handlers (Ledger.Application assembly)
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Ledger.Application.Events.IDomainEventDispatcher).Assembly));

// Domain Event Dispatcher
builder.Services.AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();

// AutoMapper
builder.Services.AddAutoMapper(
    typeof(InfrastructureProfile),
    typeof(ApplicationProfile));

// Repositories
builder.Services.AddScoped<ICofreRepository, CofreRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IParticipanteRepository, ParticipanteRepository>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<IDespesaRepository, DespesaRepository>();
builder.Services.AddScoped<IDespesaPeriodoRepository, DespesaPeriodoRepository>();
builder.Services.AddScoped<IMovimentacaoRepository, MovimentacaoRepository>();
builder.Services.AddScoped<IConviteRepository, ConviteRepository>();
builder.Services.AddScoped<IArquivoRepository, ArquivoRepository>();
builder.Services.AddScoped<IReceitaRepository, ReceitaRepository>();
builder.Services.AddScoped<IReceitaTemplateRepository, ReceitaTemplateRepository>();
builder.Services.AddScoped<IGrupoRepository, GrupoRepository>();
builder.Services.AddScoped<IConviteGrupoRepository, ConviteGrupoRepository>();


// Infrastructure Services
builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddSingleton<IBrowserService, BrowserService>();

// JWT Authentication (desabilita cookie do Identity — API pura)
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var jwtKey = builder.Configuration["Jwt:Key"]!;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = builder.Configuration["Jwt:Issuer"],
            ValidAudience            = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew                = TimeSpan.Zero,
        };
    });

builder.Services.AddAuthorization();

// CORS para Angular
builder.Services.AddCors(options =>
    options.AddPolicy("AngularApp", policy =>
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => options.WithTitle("Ledger API"));
}

app.UseCors("AngularApp");

// Handler global para erros de validação de domínio → 422
app.UseExceptionHandler(errApp => errApp.Run(async ctx =>
{
    var ex = ctx.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;
    if (ex is DomainValidationException validation)
    {
        ctx.Response.StatusCode = 422;
        ctx.Response.ContentType = "application/json";
        await ctx.Response.WriteAsJsonAsync(new { errors = validation.Errors });
    }
    else
    {
        ctx.Response.StatusCode = 500;
        ctx.Response.ContentType = "application/json";
        await ctx.Response.WriteAsJsonAsync(new { error = "Erro interno do servidor." });
    }
}));

app.UseStaticFiles(); // serve wwwroot/boletos/*.pdf
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Aplica migrations pendentes ao iniciar
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LedgerDbContext>();
    db.Database.Migrate();
}

app.Run();
