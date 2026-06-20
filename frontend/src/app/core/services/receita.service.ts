import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthService } from './auth.service';
import {
  AtualizarReceitaTemplateRequest,
  CriarReceitaRequest,
  CriarReceitaTemplateRequest,
  GerarReceitasMesRequest,
  ReceitaResponse,
  ReceitaTemplateResponse,
} from '../models/receita.model';

@Injectable({ providedIn: 'root' })
export class ReceitaService {
  private readonly http = inject(HttpClient);
  private readonly auth = inject(AuthService);
  private readonly base = `${environment.apiUrl}/api/receita`;

  private get usuarioId(): string {
    return this.auth.currentUser()!.usuarioId;
  }

  // ── Receitas ────────────────────────────────────────────────────────────────

  listar(competencia?: Date): Observable<ReceitaResponse[]> {
    const params: Record<string, string> = {};
    if (competencia) {
      params['competencia'] = competencia.toISOString();
    }
    return this.http.get<ReceitaResponse[]>(`${this.base}/${this.usuarioId}`, { params });
  }

  criar(request: CriarReceitaRequest): Observable<{ id: string }> {
    return this.http.post<{ id: string }>(this.base, request);
  }

  deletar(id: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`);
  }

  // ── Templates ───────────────────────────────────────────────────────────────

  listarTemplates(): Observable<ReceitaTemplateResponse[]> {
    return this.http.get<ReceitaTemplateResponse[]>(`${this.base}/templates/${this.usuarioId}`);
  }

  criarTemplate(request: CriarReceitaTemplateRequest): Observable<ReceitaTemplateResponse> {
    return this.http.post<ReceitaTemplateResponse>(`${this.base}/templates`, request);
  }

  atualizarTemplate(id: string, request: AtualizarReceitaTemplateRequest): Observable<ReceitaTemplateResponse> {
    return this.http.put<ReceitaTemplateResponse>(`${this.base}/templates/${id}`, request);
  }

  deletarTemplate(id: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/templates/${id}`);
  }

  gerarDoMes(competencia: Date): Observable<ReceitaResponse[]> {
    const request: GerarReceitasMesRequest = {
      usuarioId: this.usuarioId,
      competencia: competencia.toISOString(),
    };
    return this.http.post<ReceitaResponse[]>(`${this.base}/templates/gerar`, request);
  }
}
