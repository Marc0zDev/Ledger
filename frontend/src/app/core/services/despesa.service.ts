import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  DespesaResponse,
  CriarDespesaRequest,
  AtualizarDespesaRequest,
} from '../models/cofre.model';

@Injectable({ providedIn: 'root' })
export class DespesaService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/api/despesas`;

  listar(): Observable<DespesaResponse[]> {
    return this.http.get<DespesaResponse[]>(this.base);
  }

  obter(id: string): Observable<DespesaResponse> {
    return this.http.get<DespesaResponse>(`${this.base}/${id}`);
  }

  criar(request: CriarDespesaRequest): Observable<DespesaResponse> {
    return this.http.post<DespesaResponse>(this.base, request);
  }

  atualizar(id: string, request: AtualizarDespesaRequest): Observable<DespesaResponse> {
    return this.http.put<DespesaResponse>(`${this.base}/${id}`, request);
  }

  desativar(id: string): Observable<DespesaResponse> {
    return this.http.patch<DespesaResponse>(`${this.base}/${id}/desativar`, {});
  }

  deletar(id: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`);
  }
}

