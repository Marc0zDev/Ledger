export interface ReceitaResponse {
  id: string;
  nome: string;
  valor: number;
  descricao?: string;
  arquivoId?: string;
  dataRecebimento: string;
  competencia: string;
  receitaTemplateId?: string;
  createdAt: string;
}

export interface ReceitaTemplateResponse {
  id: string;
  nome: string;
  valor: number;
  descricao?: string;
  ativa: boolean;
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
}

export interface CriarReceitaTemplateRequest {
  usuarioId: string;
  nome: string;
  valor: number;
  descricao?: string;
}

export interface AtualizarReceitaTemplateRequest {
  nome: string;
  valor: number;
  descricao?: string;
}

export interface GerarReceitasMesRequest {
  usuarioId: string;
  competencia: string;
}
