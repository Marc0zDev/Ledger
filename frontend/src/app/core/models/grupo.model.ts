export interface GrupoMembroResponse {
  id: string;
  grupoId: string;
  usuarioId: string;
  nome: string;
  email: string;
  role: string; // 'Chefe' | 'Membro'
  createdAt: string;
}

export interface GrupoResponse {
  id: string;
  nome: string;
  descricao?: string;
  criadoPorUsuarioId: string;
  createdAt: string;
  updatedAt?: string;
  membros: GrupoMembroResponse[];
}

export interface CriarGrupoRequest {
  nome: string;
  descricao?: string;
}

export interface AtualizarGrupoRequest {
  nome: string;
  descricao?: string;
}

export interface AdicionarMembroGrupoRequest {
  usuarioId: string;
}

export interface ConviteGrupoResponse {
  id: string;
  grupoId: string;
  grupoNome: string;
  token: string;
  status: string;
  expiresAt: string;
  createdAt: string;
}
