import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  DespesaPeriodoResponse,
  CriarDespesaPeriodoRequest,
  AtualizarDespesaPeriodoRequest,
  PagarDespesaPeriodoRequest,
} from '../models/cofre.model';

@Injectable({ providedIn: 'root' })
export class DespesaPeriodoService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/api/despesas-periodo`;

  listarPorCompetencia(competencia: string): Observable<DespesaPeriodoResponse[]> {
    const params = new HttpParams().set('competencia', competencia);
    return this.http.get<DespesaPeriodoResponse[]>(this.base, { params });
  }

  obter(id: string): Observable<DespesaPeriodoResponse> {
    return this.http.get<DespesaPeriodoResponse>(`${this.base}/${id}`);
  }

  criar(request: CriarDespesaPeriodoRequest): Observable<DespesaPeriodoResponse> {
    return this.http.post<DespesaPeriodoResponse>(this.base, request);
  }

  atualizar(id: string, request: AtualizarDespesaPeriodoRequest): Observable<DespesaPeriodoResponse> {
    return this.http.put<DespesaPeriodoResponse>(`${this.base}/${id}`, request);
  }

  pagar(id: string, request: PagarDespesaPeriodoRequest = {}): Observable<DespesaPeriodoResponse> {
    return this.http.patch<DespesaPeriodoResponse>(`${this.base}/${id}/pagar`, request);
  }

  uploadBoleto(id: string, file: File): Observable<DespesaPeriodoResponse> {
    const form = new FormData();
    form.append('arquivo', file);
    return this.http.post<DespesaPeriodoResponse>(`${this.base}/${id}/boleto`, form);
  }

  uploadComprovante(id: string, file: File): Observable<DespesaPeriodoResponse> {
    const form = new FormData();
    form.append('arquivo', file);
    return this.http.post<DespesaPeriodoResponse>(`${this.base}/${id}/comprovante`, form);
  }

  deletar(id: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`);
  }

  gerarPeriodo(competencia: string): Observable<DespesaPeriodoResponse[]> {
    const params = new HttpParams().set('competencia', competencia);
    return this.http.post<DespesaPeriodoResponse[]>(`${this.base}/gerar`, null, { params });
  }

  gerarRelatorioMensal(competencia: string): Observable<Blob> {
    const params = new HttpParams().set('competencia', competencia);
    return this.http.get(`${environment.apiUrl}/api/relatorio/mensal`, { params, responseType: 'blob' });
  }
}
