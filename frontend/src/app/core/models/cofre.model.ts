// ── Cofre ────────────────────────────────────────────────────────────────

export interface CriarCofreRequest {
  nome: string;
  meta: number;
  descricao?: string;
  categoria?: string;
}

export interface AtualizarCofreRequest {
  nome: string;
  meta: number;
  descricao?: string;
  categoria?: string;
}

export interface MovimentacaoResponse {
  id: string;
  cofreId: string;
  usuarioId: string;
  descricao: string;
  valor: number;
  tipo: string;
  createdAt: string;
}

export interface CofreResponse {
  id: string;
  nome: string;
  descricao?: string;
  meta: number;
  status: string;
  categoria: string;
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

export interface ParticipanteResponse {
  id: string;
  cofreId: string;
  usuarioId: string;
  nome: string;
  email: string;
  createdAt: string;
}

// ── Despesa ──────────────────────────────────────────────────────────────

export interface RegistrarDespesaRequest {
  descricao: string;
  valor: number;
  dataVencimento: string;
  categoria?: number;
  recorrente?: boolean;
}

export interface AtualizarDespesaRequest {
  descricao: string;
  valor: number;
  dataVencimento: string;
  categoria?: number;
  recorrente?: boolean;
}

export interface DespesaResponse {
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
