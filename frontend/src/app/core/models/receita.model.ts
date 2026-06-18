export interface ReceitaResponse {
  id: string;
  nome: string;
  valor: number;
  descricao?: string;
  arquivoId?: string;
  dataRecebimento: string;
  competencia: string;
  receitaTemplateId?: string;
  grupoId?: string;
  usuarioNome?: string;
  createdAt: string;
}

export interface ReceitaTemplateResponse {
  id: string;
  nome: string;
  valor: number;
  descricao?: string;
  ativa: boolean;
  dataInicio: string;
  dataFim?: string;
  createdAt: string;
}

export interface CriarReceitaRequest {
  usuarioId: string;
  nome: string;
  valor: number;
  descricao?: string;
  arquivoId?: string;
  dataRecebimento: string;
  receitaTemplateId?: string;
  grupoId?: string;
}

export interface CriarReceitaTemplateRequest {
  usuarioId: string;
  nome: string;
  valor: number;
  descricao?: string;
  dataInicio: string;
  dataFim?: string;
}

export interface AtualizarReceitaTemplateRequest {
  nome: string;
  valor: number;
  descricao?: string;
  dataInicio: string;
  dataFim?: string;
}

export interface GerarReceitasMesRequest {
  usuarioId: string;
  competencia: string;
}
