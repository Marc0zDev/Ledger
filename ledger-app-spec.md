# Ledger App — Especificação de produto

> Documento gerado para compartilhamento com IA ou equipe técnica.
> Cobre arquitetura, módulos, regras de negócio e modelo de dados.

---

## Visão geral

Aplicativo financeiro pessoal com quatro pilares:

1. **Cofres** — poupança virtual com metas, multi-usuário
2. **Despesas** — controle de gastos fixos, variáveis e eventuais
3. **Boletos & Contas** — centralização de vencimentos via upload PDF ou bot automático
4. **Controle Financeiro** — dashboard que agrega tudo e responde "como estou este mês?"

---

## Módulo 1 — Cofres

### O que é
Um cofre é uma conta poupança virtual com ou sem meta financeira. Tem dono, pode ter múltiplos membros convidados, e cada membro tem um papel.

### Entidade `Vault`

| Campo | Tipo | Descrição |
|---|---|---|
| `id` | uuid | PK |
| `owner_id` | uuid FK → User | Dono do cofre |
| `name` | string | Nome do cofre |
| `description` | string? | Descrição opcional |
| `goal_amount` | decimal? | Meta em R$ (opcional) |
| `goal_deadline` | date? | Prazo alvo (opcional) |
| `balance` | decimal (computed) | Soma das transações |
| `visibility` | enum: private, shared | Visibilidade |
| `status` | enum: active, paused, completed | Estado |
| `created_at` | timestamp | — |

### Entidade `VaultMember`

| Campo | Tipo | Descrição |
|---|---|---|
| `vault_id` | uuid FK | — |
| `user_id` | uuid FK | — |
| `role` | enum: admin, contributor | Papel |
| `joined_at` | timestamp | — |

### Entidade `VaultTransaction`

| Campo | Tipo | Descrição |
|---|---|---|
| `id` | uuid | PK |
| `vault_id` | uuid FK | — |
| `user_id` | uuid FK | Quem fez |
| `type` | enum: deposit, withdrawal | Tipo |
| `amount` | decimal | Valor |
| `description` | string? | Descrição |
| `created_at` | timestamp | — |

### Regras de negócio

- O saldo (`balance`) é sempre calculado pela soma das transações — nunca editado diretamente.
- **Admin**: pode depositar, sacar, editar meta, convidar/remover membros, fechar cofre.
- **Contributor**: pode apenas depositar.
- O dono não pode ser removido do cofre.
- Quando a meta está definida, o app calcula e exibe:
  - Progresso em %
  - Valor restante
  - Projeção de data de conclusão baseada na média de aportes dos últimos 3 meses

### Fluxo de convite

1. Dono ou admin gera link de convite (token UUID com expiração de 7 dias)
2. Convidado acessa o link → aceita → entra como `contributor`
3. Admin pode promover para `admin` depois

---

## Módulo 2 — Despesas

### O que é
Mapeamento de toda saída financeira recorrente e variável para que o Controle Financeiro calcule "quanto sobra por mês".

### Tipos de despesa

| Tipo | Descrição | Comportamento |
|---|---|---|
| `fixed` | Valor fixo todo mês (aluguel, assinatura) | Cadastrada uma vez, carregada automaticamente em cada período |
| `variable` | Categoria com teto mensal (alimentação, transporte) | Usuário define teto; gastos são lançados manualmente ou via boleto |
| `one_time` | Eventual, sem recorrência | Lançada pontualmente |

### Entidade `Expense`

| Campo | Tipo | Descrição |
|---|---|---|
| `id` | uuid | PK |
| `user_id` | uuid FK | — |
| `category_id` | uuid FK | Categoria |
| `type` | enum: fixed, variable, one_time | Tipo |
| `name` | string | Nome da despesa |
| `planned_amount` | decimal | Valor planejado / teto |
| `recurrence_day` | int? | Dia do mês (para fixas) |
| `is_active` | bool | Ativa para novos períodos |

### Entidade `Category`

| Campo | Tipo | Descrição |
|---|---|---|
| `id` | uuid | PK |
| `user_id` | uuid FK | Dono (ou null = padrão do sistema) |
| `name` | string | Nome |
| `icon` | string? | Ícone |
| `color` | string? | Cor hex |

**Categorias padrão do sistema:** moradia, transporte, alimentação, saúde, educação, lazer, assinaturas, financiamentos, outros.

### Ciclo mensal de despesas

1. No início de cada período, o sistema clona todas as despesas `fixed` e `variable` ativas como entradas previstas no `PeriodExpense`.
2. O usuário confirma, ajusta valores se necessário.
3. Ao longo do mês, lança gastos variáveis manualmente ou vincula boletos.
4. No fechamento do período, o sistema consolida realizado vs planejado por categoria.

### Entidade `PeriodExpense`

| Campo | Tipo | Descrição |
|---|---|---|
| `id` | uuid | PK |
| `period_id` | uuid FK | Período mensal |
| `expense_id` | uuid FK? | Despesa de origem (null = avulsa) |
| `category_id` | uuid FK | — |
| `planned_amount` | decimal | Valor planejado |
| `actual_amount` | decimal | Valor realizado |
| `paid_at` | timestamp? | Data de pagamento |
| `bill_id` | uuid FK? | Boleto vinculado (opcional) |

---

## Módulo 3 — Boletos & Contas

### O que é
Centralização de tudo que tem vencimento e precisa ser pago. Entrada via upload PDF manual ou bot automático.

### Entidade `Bill`

| Campo | Tipo | Descrição |
|---|---|---|
| `id` | uuid | PK |
| `user_id` | uuid FK | — |
| `category_id` | uuid FK? | Categoria vinculada |
| `source` | enum: manual, bot_email, bot_portal, bot_api | Origem |
| `beneficiary` | string | Beneficiário/emissor |
| `amount` | decimal | Valor |
| `due_date` | date | Vencimento |
| `barcode` | string? | Linha digitável / código de barras |
| `pdf_url` | string? | Arquivo original |
| `status` | enum: pending, due_soon, overdue, paid, cancelled | Estado |
| `paid_at` | timestamp? | Data de pagamento |
| `period_expense_id` | uuid FK? | Vinculado a despesa do período |

### Estados do boleto

```
pending → due_soon (< 5 dias para vencer)
        → overdue  (passou do vencimento sem pagamento)
        → paid     (marcado como pago)
        → cancelled (cancelado manualmente)
```

### Upload de PDF

1. Usuário faz upload do PDF.
2. Sistema extrai via OCR: beneficiário, valor, vencimento, código de barras.
3. Usuário vê os dados extraídos e confirma (ou corrige).
4. Boleto entra na fila com status `pending`.

### Entidade `BotConfig`

| Campo | Tipo | Descrição |
|---|---|---|
| `id` | uuid | PK |
| `user_id` | uuid FK | — |
| `source_type` | enum: email, portal, open_finance | Tipo de fonte |
| `label` | string | Nome amigável (ex: "Gmail pessoal") |
| `credentials_encrypted` | text | Credenciais criptografadas (AES-256) |
| `frequency` | enum: daily, weekly | Frequência de busca |
| `run_at` | time | Horário de execução |
| `last_run_at` | timestamp? | Última execução |
| `last_run_status` | enum: ok, error, partial | Status |
| `is_active` | bool | — |

### Fontes do bot

| Fonte | Mecanismo | Critério de busca |
|---|---|---|
| E-mail (IMAP/OAuth) | Busca anexos PDF | Palavras-chave: "boleto", "fatura", "NF", "vencimento" |
| Portal web | Scraping autenticado | Credenciais do portal; lista de contas/faturas |
| Open Finance | API bancária | Contas a pagar via Open Finance BR |

### Vinculação com despesas
Um boleto pode ser vinculado a uma `PeriodExpense`, alimentando o `actual_amount` automaticamente.

---

## Módulo 4 — Controle Financeiro

### O que é
O módulo núcleo que agrega todos os outros e responde: *"como estou financeiramente este mês?"*

### Entidade `Period`

| Campo | Tipo | Descrição |
|---|---|---|
| `id` | uuid | PK |
| `user_id` | uuid FK | — |
| `month` | int | Mês (1–12) |
| `year` | int | Ano |
| `declared_income` | decimal | Renda declarada para o período |
| `status` | enum: open, closing, closed, archived | Estado |
| `opened_at` | timestamp | — |
| `closed_at` | timestamp? | — |

### Entidade `IncomeEntry`

| Campo | Tipo | Descrição |
|---|---|---|
| `id` | uuid | PK |
| `period_id` | uuid FK | — |
| `source` | string | Origem (salário, freelance, aluguel…) |
| `amount` | decimal | Valor |
| `received_at` | date | Data de recebimento |

### Fórmula central

```
total_saidas = despesas_pagas + boletos_pagos + aportes_cofres - saques_cofres

saldo_disponivel = renda - total_saidas

comprometimento_pct = (total_saidas / renda) * 100

ritmo_diario = total_saidas_ate_hoje / dias_passados_no_periodo

projecao_sobra = renda - (ritmo_diario * dias_totais_do_mes)
```

**Nota sobre projeção mais precisa:**
As despesas fixas já estão lançadas no período com valor conhecido. A projeção de ritmo aplica-se apenas ao componente variável:

```
total_previsto_fixo = soma das PeriodExpenses do tipo fixed
total_variavel_ate_hoje = total_saidas - total_previsto_fixo
ritmo_variavel = total_variavel_ate_hoje / dias_passados
projecao_variavel_restante = ritmo_variavel * dias_restantes
projecao_sobra = renda - total_previsto_fixo - total_variavel_ate_hoje - projecao_variavel_restante
```

### Classificação de saúde financeira

| Comprometimento | Status | Cor |
|---|---|---|
| < 60% | Saudável | Verde |
| 60% – 80% | Atenção | Amarelo |
| > 80% | Alto | Vermelho |

### Separação de saídas no dashboard

| Tipo | Natureza | Exibição |
|---|---|---|
| Despesas pagas | Gasto consumido | Comprometimento |
| Boletos pagos | Gasto consumido | Comprometimento |
| Aporte em cofre | Saída alocada (positiva) | Separado — "dinheiro guardado" |
| Saque de cofre | Retorno ao saldo | Reduz saídas |

> **Regra de UX importante:** a separação visual entre "gastos" e "aportes em cofre" é fundamental. Um comprometimento de 75% com 15% sendo aporte em cofre é situação saudável — o dashboard deve comunicar isso claramente, sem gerar ansiedade desnecessária.

### Ciclo de vida do período

```
open → closing (últimos 3 dias do mês, ou acionado manualmente)
     → closed  (ao fechar o período)
     → archived (após N meses, para limpeza de dados ativos)
```

Ao abrir um período:
1. Sistema clona despesas `fixed` e `variable` ativas como `PeriodExpense` com `planned_amount`.
2. Usuário confirma e ajusta se necessário.

Ao fechar um período:
1. Sistema consolida `actual_amount` vs `planned_amount` por categoria.
2. Gera o `PeriodReport` com resumo.
3. Dispara notificação de fechamento com resumo.

### Entidade `PeriodReport` (gerado ao fechar)

| Campo | Tipo | Descrição |
|---|---|---|
| `id` | uuid | PK |
| `period_id` | uuid FK | — |
| `total_income` | decimal | Renda total do período |
| `total_expenses` | decimal | Total de despesas pagas |
| `total_bills` | decimal | Total de boletos pagos |
| `total_vault_deposits` | decimal | Total aportado em cofres |
| `total_vault_withdrawals` | decimal | Total sacado de cofres |
| `final_balance` | decimal | Saldo real ao fechar |
| `commitment_pct` | decimal | % comprometido |
| `category_breakdown` | jsonb | Array: { category, planned, actual, diff } |

---

## Módulo 5 — Notificações & Alertas

### Gatilhos e regras

| Evento | Condição | Canal |
|---|---|---|
| Boleto vencendo | N dias antes do vencimento (padrão: 3, configurável) | Push + e-mail |
| Boleto vencido | Passou da data sem `paid_at` | Push + e-mail |
| Bot encontrou boleto | Novo boleto extraído pelo bot | Push |
| Meta de cofre atingida | `balance >= goal_amount` | Push + e-mail |
| Teto de categoria ultrapassado | `actual_amount > planned_amount` na categoria | Push |
| Ritmo de ultrapassagem | Projeção indica ultrapassagem em ≤ N dias | Push |
| Anomalia de gasto | Categoria X subiu > 20% vs mês anterior | Push |
| Período fechado | Ao consolidar o mês | Push + e-mail (com resumo) |
| Convite de cofre | Ao receber convite | Push + e-mail |

### Regras de anomalia detalhadas

```
anomalia_dispara SE:
  actual_amount_categoria_mes_atual > actual_amount_categoria_mes_anterior * 1.20
  E categoria tem ao menos 2 meses de histórico
  E alerta não foi silenciado pelo usuário neste mês
```

Limiar de 20% é configurável pelo usuário por categoria. O usuário pode silenciar um alerta específico ("já sei, é uma compra pontual") sem desativar a regra.

### Entidade `Notification`

| Campo | Tipo | Descrição |
|---|---|---|
| `id` | uuid | PK |
| `user_id` | uuid FK | — |
| `type` | string | Tipo do gatilho |
| `payload` | jsonb | Dados do evento |
| `read_at` | timestamp? | — |
| `dismissed_at` | timestamp? | Silenciado |
| `created_at` | timestamp | — |

---

## Modelo de dados — visão geral de relações

```
User
  ├── Vault (owner)            1 → N
  │     ├── VaultMember        N → N (com User)
  │     └── VaultTransaction   1 → N
  ├── Period                   1 → N
  │     ├── IncomeEntry        1 → N
  │     ├── PeriodExpense      1 → N
  │     └── PeriodReport       1 → 1
  ├── Expense                  1 → N
  │     └── Category           N → 1
  ├── Bill                     1 → N
  │     └── BotConfig          N → 1 (fonte)
  └── Notification             1 → N

PeriodExpense
  ├── → Expense (origem, opcional)
  ├── → Bill (boleto vinculado, opcional)
  └── → Category

Bill
  └── → PeriodExpense (vinculação, opcional)
```

---

## Fluxo do bot de boletos (detalhado)

```
1. BotConfig.run_at dispara o job agendado
2. Bot autentica na fonte (email IMAP / portal / Open Finance)
3. Busca documentos candidatos (PDFs com palavras-chave)
4. Para cada PDF:
   a. Extrai campos via OCR: beneficiário, valor, vencimento, código de barras
   b. Verifica se já existe Bill com mesmo barcode → ignora duplicata
   c. Cria Bill com status "pending" e source = bot_*
   d. Dispara notificação "Novo boleto encontrado"
5. Atualiza BotConfig.last_run_at e last_run_status
6. Se erro parcial: status = "partial", registra quais fontes falharam
```

---

## Considerações de segurança

- Credenciais do bot armazenadas criptografadas (AES-256) — nunca em texto plano.
- Chave de criptografia gerenciada separadamente (ex: AWS KMS ou Vault).
- Links de convite de cofre: token UUID v4, expiração de 7 dias, uso único.
- PDFs de boletos: armazenados em bucket privado com acesso via URL assinada com TTL curto.
- Open Finance: usar fluxo OAuth padrão do Open Finance BR, sem armazenar credenciais bancárias.

---

## Próximos passos sugeridos

- [ ] Detalhar o ERD completo em formato de migração (SQL ou Prisma schema)
- [ ] Especificar a API REST (endpoints, payloads, autenticação)
- [ ] Definir regras de permissão por papel (RBAC) para cada endpoint
- [ ] Especificar o worker do bot (fila, retry, dead-letter)
- [ ] Wireframes das telas principais (cofres, dashboard, upload de boleto)
- [ ] Definir estratégia de sincronização offline (app mobile)
