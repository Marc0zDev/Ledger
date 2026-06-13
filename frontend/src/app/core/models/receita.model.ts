export interface ReceitaResponse {
  id: string;
  nome: string;
  valor: number;
  descricao: string;
  arquivoId?: string;
  dataRecebimento: string;
  dataCriacao: string;
}

export interface CriarReceitaRequest {
  usuarioId: string;
  nome: string;
  valor: number;
  descricao: string;
  dataRecebimento: string;
}
