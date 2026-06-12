// ── Cofre ────────────────────────────────────────────────────────────────

export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  total: number;
  totalPages: number;
  hasPrev: boolean;
  hasNext: boolean;
}

export interface CriarCofreRequest {
  nome: string;
  meta: number;
  descricao?: string;
  categoria?: string;
  visibilidade?: string; // 'Privado' | 'Compartilhado'
}

export interface AtualizarCofreRequest {
  nome: string;
  meta: number;
  descricao?: string;
  categoria?: string;
  visibilidade?: string; // 'Privado' | 'Compartilhado'
}

export interface MovimentacaoResponse {
  id: string;
  cofreId: string;
  usuarioId: string;
  usuarioNome?: string;
  descricao: string;
  valor: number;
  tipo: string; // 'Entrada' | 'Saida' — may be missing on legacy records
  data: string;
  createdAt: string;
}

export interface RegistrarMovimentacaoRequest {
  descricao: string;
  valor: number;
  tipo: string; // 'Entrada' | 'Saida'
  data: string;
}

export interface CofreResponse {
  id: string;
  nome: string;
  descricao?: string;
  meta: number;
  status: string;
  categoria: string;
  visibilidade: string; // 'Privado' | 'Compartilhado'
  totalMovimentado: number;
  createdAt: string;
  updatedAt?: string;
  participantes: ParticipanteResponse[];
  movimentacoes: MovimentacaoResponse[];
}

// ── Participante (membership) ──────────────────────────────────────────────

export interface AdicionarParticipanteRequest {
  usuarioId: string;
}

export interface AlterarRoleRequest {
  role: string; // 'Admin' | 'Contributor'
}

export interface ParticipanteResponse {
  id: string;
  cofreId: string;
  usuarioId: string;
  nome: string;
  email: string;
  role: string; // 'Admin' | 'Contributor'
  createdAt: string;
}

// ── Categoria ────────────────────────────────────────────────────────────

export interface CategoriaResponse {
  id: string;
  nome: string;
  icone?: string;
  cor?: string;
  isSystem: boolean;
}

// ── Despesa (template) ────────────────────────────────────────────────────

export type TipoDespesa = 1 | 2 | 3; // Fixa=1, Variavel=2, Avulsa=3

export interface CriarDespesaRequest {
  nome: string;
  tipo: TipoDespesa;
  valorPlanejado: number;
  categoriaId: string;
  diaVencimento?: number;
}

export interface AtualizarDespesaRequest {
  nome: string;
  tipo: TipoDespesa;
  valorPlanejado: number;
  categoriaId: string;
  diaVencimento?: number;
}

export interface DespesaResponse {
  id: string;
  nome: string;
  tipo: string;
  valorPlanejado: number;
  diaVencimento?: number;
  ativa: boolean;
  arquivoId?: string;
  categoriaId: string;
  categoriaNome: string;
  categoriaIcone?: string;
  categoriaCor?: string;
  usuarioId: string;
  createdAt: string;
  updatedAt?: string;
}

// ── DespesaPeriodo ────────────────────────────────────────────────────────

export interface CriarDespesaPeriodoRequest {
  despesaId?: string;
  categoriaId: string;
  descricao: string;
  valorPlanejado: number;
  competencia: string;
}

export interface AtualizarDespesaPeriodoRequest {
  descricao: string;
  valorPlanejado: number;
  categoriaId: string;
}

export interface PagarDespesaPeriodoRequest {
  dataPagamento?: string;
  valorRealizado?: number;
}

export interface DespesaPeriodoResponse {
  id: string;
  despesaId?: string;
  arquivoId?: string;
  descricao: string;
  categoriaId: string;
  categoriaNome: string;
  categoriaIcone?: string;
  categoriaCor?: string;
  usuarioId: string;
  valorPlanejado: number;
  valorRealizado: number;
  paga: boolean;
  pagaEm?: string;
  boletoUrl?: string;
  competencia: string;
  createdAt: string;
  updatedAt?: string;
}

// ── Cofre Despesa (movimentações de cofre – legacy) ───────────────────────

export interface RegistrarDespesaRequest {
  descricao: string;
  valor: number;
  dataVencimento: string;
  categoria?: number;
  recorrente?: boolean;
}

export interface CofreDespesaResponse {
  id: string;
  descricao: string;
  valor: number;
  dataVencimento: string;
  dataPagamento?: string;
  paga: boolean;
  boletoUrl?: string;
  usuarioId: string;
  categoria: string;
  recorrente: boolean;
  createdAt: string;
  updatedAt?: string;
}

// ── Usuário ──────────────────────────────────────────────────────────────

export interface CriarUsuarioRequest {
  nome: string;
  email: string;
}

export interface AtualizarUsuarioRequest {
  nome: string;
  email: string;
}

export interface UsuarioResponse {
  id: string;
  nome: string;
  email: string;
  createdAt: string;
  updatedAt?: string;
}