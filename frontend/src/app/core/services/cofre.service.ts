import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  CofreResponse, CriarCofreRequest, AtualizarCofreRequest,
  AdicionarParticipanteRequest, ParticipanteResponse,
  RegistrarDespesaRequest, DespesaResponse,
} from '../models/cofre.model';

@Injectable({ providedIn: 'root' })
export class CofreService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/api/cofres`;

  // ── Cofres ────────────────────────────────────────────────────────────

  listar(): Observable<CofreResponse[]> {
    return this.http.get<CofreResponse[]>(this.base);
  }

  obterPorId(id: string): Observable<CofreResponse> {
    return this.http.get<CofreResponse>(`${this.base}/${id}`);
  }

  obterComDetalhes(id: string): Observable<CofreResponse> {
    return this.http.get<CofreResponse>(`${this.base}/${id}/detalhes`);
  }

  criar(request: CriarCofreRequest): Observable<CofreResponse> {
    return this.http.post<CofreResponse>(this.base, request);
  }

  atualizar(id: string, request: AtualizarCofreRequest): Observable<CofreResponse> {
    return this.http.put<CofreResponse>(`${this.base}/${id}`, request);
  }

  deletar(id: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`);
  }

  // ── Participantes (membership) ─────────────────────────────────────────

  listarParticipantes(cofreId: string): Observable<ParticipanteResponse[]> {
    return this.http.get<ParticipanteResponse[]>(`${this.base}/${cofreId}/participantes`);
  }

  adicionarParticipante(cofreId: string, request: AdicionarParticipanteRequest): Observable<ParticipanteResponse> {
    return this.http.post<ParticipanteResponse>(`${this.base}/${cofreId}/participantes`, request);
  }

  removerParticipante(cofreId: string, participanteId: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/${cofreId}/participantes/${participanteId}`);
  }

  // ── Despesas ─────────────────────────────────────────────────────────

  listarDespesas(cofreId: string): Observable<DespesaResponse[]> {
    return this.http.get<DespesaResponse[]>(`${environment.apiUrl}/api/despesas/cofre/${cofreId}`);
  }

  registrarDespesa(cofreId: string, request: RegistrarDespesaRequest): Observable<DespesaResponse> {
    return this.http.post<DespesaResponse>(`${environment.apiUrl}/api/despesas`, { ...request, cofreId });
  }

  removerDespesa(cofreId: string, despesaId: string): Observable<void> {
    return this.http.delete<void>(`${environment.apiUrl}/api/despesas/${despesaId}`);
  }
}
