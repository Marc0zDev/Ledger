import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  GrupoResponse, CriarGrupoRequest, AtualizarGrupoRequest,
  AdicionarMembroGrupoRequest, GrupoMembroResponse,
} from '../models/grupo.model';
import { DespesaPeriodoResponse } from '../models/cofre.model';
import { ReceitaResponse } from '../models/receita.model';

@Injectable({ providedIn: 'root' })
export class GrupoService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/api/grupos`;

  listar(): Observable<GrupoResponse[]> {
    return this.http.get<GrupoResponse[]>(this.base);
  }

  obterPorId(id: string): Observable<GrupoResponse> {
    return this.http.get<GrupoResponse>(`${this.base}/${id}`);
  }

  criar(request: CriarGrupoRequest): Observable<GrupoResponse> {
    return this.http.post<GrupoResponse>(this.base, request);
  }

  atualizar(id: string, request: AtualizarGrupoRequest): Observable<GrupoResponse> {
    return this.http.put<GrupoResponse>(`${this.base}/${id}`, request);
  }

  deletar(id: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`);
  }

  adicionarMembro(grupoId: string, request: AdicionarMembroGrupoRequest): Observable<GrupoMembroResponse> {
    return this.http.post<GrupoMembroResponse>(`${this.base}/${grupoId}/membros`, request);
  }

  removerMembro(grupoId: string, membroId: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/${grupoId}/membros/${membroId}`);
  }

  listarDespesas(grupoId: string, competencia: string): Observable<DespesaPeriodoResponse[]> {
    return this.http.get<DespesaPeriodoResponse[]>(`${this.base}/${grupoId}/despesas`, { params: { competencia } });
  }

  listarReceitas(grupoId: string, competencia: string): Observable<ReceitaResponse[]> {
    return this.http.get<ReceitaResponse[]>(`${this.base}/${grupoId}/receitas`, { params: { competencia } });
  }
}
