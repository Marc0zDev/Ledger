import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  DespesaResponse,
  RegistrarDespesaRequest,
  AtualizarDespesaRequest,
} from '../models/cofre.model';

@Injectable({ providedIn: 'root' })
export class DespesaService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/api/despesas`;

  listar(): Observable<DespesaResponse[]> {
    return this.http.get<DespesaResponse[]>(this.base);
  }

  pendentes(vencimentoAte?: string): Observable<DespesaResponse[]> {
    let params = new HttpParams();
    if (vencimentoAte) params = params.set('vencimentoAte', vencimentoAte);
    return this.http.get<DespesaResponse[]>(`${this.base}/pendentes`, { params });
  }

  porCofre(cofreId: string): Observable<DespesaResponse[]> {
    return this.http.get<DespesaResponse[]>(`${this.base}/cofre/${cofreId}`);
  }

  obter(id: string): Observable<DespesaResponse> {
    return this.http.get<DespesaResponse>(`${this.base}/${id}`);
  }

  criar(request: RegistrarDespesaRequest): Observable<DespesaResponse> {
    return this.http.post<DespesaResponse>(this.base, request);
  }

  atualizar(id: string, request: AtualizarDespesaRequest): Observable<DespesaResponse> {
    return this.http.put<DespesaResponse>(`${this.base}/${id}`, request);
  }

  pagar(id: string, dataPagamento?: string): Observable<DespesaResponse> {
    return this.http.patch<DespesaResponse>(`${this.base}/${id}/pagar`, { dataPagamento });
  }

  uploadBoleto(id: string, file: File): Observable<DespesaResponse> {
    const form = new FormData();
    form.append('boleto', file);
    return this.http.post<DespesaResponse>(`${this.base}/${id}/boleto`, form);
  }

  deletar(id: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`);
  }
}
