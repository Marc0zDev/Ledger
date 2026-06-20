# Ledger — Contexto do Projeto

## Stack

| Camada | Tecnologia |
|---|---|
| Backend | ASP.NET Core 9, C#, PostgreSQL (Npgsql) |
| ORM | Entity Framework Core |
| Mediação | MediatR (CQRS) |
| Mapeamento | AutoMapper |
| Autenticação | ASP.NET Identity + JWT Bearer |
| Frontend | Angular 21, standalone components, Signals |
| UI Library | PrimeNG 21 (tema Aura, light only) |
| Icons | PrimeIcons |

---

## Arquitetura Backend

### Camadas
```
Ledger.Api           → Controllers (thin, só despacham via IMediator)
Ledger.Application   → Commands, Queries, DTOs, Events, Profiles
Ledger.Domain        → Entities, Interfaces, Enums, Events, Exceptions
Ledger.Infrastructure → Repositories, DbContext, Models, Services externos
```

### Regra de ouro — CQRS completo
- **Controllers injetam apenas `IMediator`**. Nunca injetam services de Application.
- Toda lógica de negócio vai em Command/Query handlers.
- `IEmailService` e `IIdentityService` são serviços de infraestrutura (permitidos nos handlers).
- Domain Events são despachados via `IDomainEventDispatcher` após persistência.

### Padrão de arquivo Command/Query
Command e Handler no mesmo arquivo, separados por comentário:
```csharp
// ── Command ───────────────────────────────────────────────────────────────────
public record CriarXCommand(...) : IRequest<XResponse>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class CriarXCommandHandler : IRequestHandler<CriarXCommand, XResponse> { ... }
```

### Exceções de domínio
- `DomainValidationException` → retorna HTTP 422 via global exception handler em `Program.cs`.
- Validações usam `Flunt.Notifications` via `BaseDomain`.

---

## Arquitetura Frontend

### Estrutura
```
src/app/
  core/
    guards/        → auth.guard.ts
    interceptors/  → auth.interceptor.ts (JWT automático)
    models/        → interfaces TypeScript
    services/      → todos os serviços injetáveis
  features/        → uma pasta por domínio (despesas, cofres, auth, ...)
  shared/
    components/    → layout, sidebar, topbar, ledger-table, file-viewer
```

### Serviços globais chave
| Serviço | Uso |
|---|---|
| `NotificationService` | Toasts — `.success()`, `.error()`, `.warn()`, `.info()` |
| `FileViewerService` | Abre arquivos em modal — `.open(arquivoId)` |
| `ArquivoService` | Upload e download de arquivos |

### Regras Angular
- Componentes **standalone** com `imports: []` explícito.
- Estado via **Signals** (`signal()`, `computed()`). Não usar `BehaviorSubject` para estado local.
- Erros e mensagens de sucesso sempre via `NotificationService` (toast). Nunca exibir `<p class="form-erro">` inline para feedback de ações.
- Arquivo aberto sempre via `FileViewerService.open()`. Nunca usar `window.open()`.

### LedgerTableComponent
Componente genérico de tabela. Usar sempre que listar dados tabulares.
- Suporta: `currency`, `date`, `tag`, `actions` (kebab menu)
- Input `[options]` para colunas, `[dataSource]` para dados
- Output `(rowAction)` para ações por linha

---

## Design System (Frontend)

### Paleta de cores (CSS variables em `styles.scss`)

#### Superfícies
```scss
--bg:          oklch(0.920 0.010 70)  /* fundo da página — bege claro */
--surface:     #fff                   /* cards, painéis */
--surface-alt: oklch(0.880 0.010 65)  /* sidebar, topbar */
--border:      oklch(0.885 0.008 70)  /* bordas sutis */
--border-strong: oklch(0.800 0.010 65)
```

#### Texto
```scss
--ink:   oklch(0.155 0.012 55)  /* texto principal — marrom escuro */
--muted: oklch(0.450 0.012 55)  /* texto secundário */
--faint: oklch(0.660 0.008 60)  /* placeholders, hints */
```

#### Marca e semântica
```scss
--gold:              oklch(0.560 0.110 65)      /* cor primária — dourado */
--gold-subtle:       oklch(0.560 0.110 65 / 0.10)
--error:             oklch(0.520 0.180 25)       /* vermelho */
--color-success:     oklch(0.50 0.112 134)       /* verde */
--color-success-subtle: oklch(0.80 0.06 134)
--color-warn:        oklch(0.50 0.112 65)
--color-warn-subtle: oklch(0.80 0.06 65)
--focus-ring:        oklch(0.560 0.110 65 / 0.35)
```

### Tipografia

| Uso | Fonte | Peso | Tamanho |
|---|---|---|---|
| Títulos de página | Cormorant Garamond | 400 | clamp(1.75rem, 3vw, 2.25rem) |
| Subtítulos / seções | Cormorant Garamond | 400 | 1.25–1.5rem |
| Body / UI | DM Sans | 400–600 | 0.875rem |
| Labels de form | DM Sans | 600 | 0.75–0.8rem, uppercase |
| Células de tabela | DM Sans | 400–500 | 0.8125–0.875rem |
| Tags / badges | DM Sans | 600 | 0.7rem, uppercase |

> Títulos usam Cormorant. Tudo funcional (labels, botões, inputs, tabelas) usa DM Sans.

### Border radius

| Contexto | Valor |
|---|---|
| Botões, inputs | 7–8px |
| Cards, painéis, tabelas | 12–14px |
| Sidebar, topbar, auth card | 18–20px |
| Tags / badges | 6px ou 999px (pill) |

### Sombras

```scss
/* Cards e painéis */
box-shadow: 0 0 0 1px rgba(0,0,0,0.09), 0 2px 8px rgba(0,0,0,0.07);

/* Hover de card */
box-shadow: 0 0 0 1px rgba(0,0,0,0.13), 0 4px 16px rgba(0,0,0,0.10);

/* Sidebar e topbar */
box-shadow: 0 2px 12px rgba(0,0,0,0.09), 0 1px 3px rgba(0,0,0,0.05);
```

### Botões

**Primário** — ação principal da página:
```scss
background: var(--ink);
color: #fff;
padding: 0.55rem 1.25rem;
border-radius: 8px;
font: 600 0.875rem 'DM Sans';
&:hover { opacity: 0.8; }
&:disabled { opacity: 0.4; cursor: default; }
```

**Secundário** — ação secundária:
```scss
background: transparent;
border: 1.5px solid var(--border-strong);
color: var(--ink);
padding: 0.5rem 1.1rem;
border-radius: 8px;
```

**Ghost / Cancelar**:
```scss
border: 1px solid var(--border);
color: var(--muted);
background: transparent;
```

**Danger** (em menus de ação):
```scss
color: var(--faint); /* inicial */
&:hover { background: oklch(0.520 0.180 25 / 0.08); color: var(--error); }
```

### Inputs e Forms

```scss
/* Input base */
border: 1px solid var(--border);
background: var(--bg);
padding: 0.5rem 0.75rem;
border-radius: 8px;
font: 400 0.875rem 'DM Sans';
color: var(--ink);

/* Focus */
border-color: var(--gold);
background: var(--surface);
box-shadow: 0 0 0 3px oklch(0.560 0.110 65 / 0.15);

/* Label */
font: 600 0.75rem 'DM Sans';
text-transform: uppercase;
letter-spacing: 0.06em;
color: var(--muted);
```

### Transições
```scss
/* Padrão para hover e estados */
transition: background 0.15s, color 0.15s, border-color 0.15s, opacity 0.15s;

/* Collapse/expand (sidebar) */
transition: width 0.25s cubic-bezier(0.4, 0, 0.2, 1);

/* Spinner */
animation: spin 0.7s linear infinite;
border-top-color: var(--gold);
```

### Padrão de card

```scss
background: var(--surface);
border: 1px solid var(--border);
border-radius: 12px;
padding: 1.5rem;
box-shadow: 0 0 0 1px rgba(0,0,0,0.09), 0 2px 8px rgba(0,0,0,0.07);
```

Accent lateral (status):
```scss
border-left: 4px solid var(--gold); /* ou --color-success, --error conforme status */
```

### Layout geral
- Sidebar: `220px` (colapsada: `64px`), `sticky top: 12px`, altura `calc(100dvh - 24px)`
- App shell: `display: flex`, fundo `var(--bg)`
- Page area: `border-radius: 18px`, overflow-y auto
- Padding interno de páginas: `2.5rem`

---

## Convenções de Commit
- Formato: `tipo(escopo): mensagem` — ex: `feat(despesas): adiciona filtro por categoria`
- Sem linha `Co-Authored-By` nos commits.
