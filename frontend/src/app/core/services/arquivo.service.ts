import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, from, map, switchMap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ArquivoRequest, ArquivoResponse, buildArquivoRequest } from '../models/arquivo.model';

@Injectable({ providedIn: 'root' })
export class ArquivoService {
  private readonly http = inject(HttpClient);
  private readonly despesasBase = `${environment.apiUrl}/api/despesas`;
  private readonly arquivosBase = `${environment.apiUrl}/api/arquivos`;

  registrar(request: ArquivoRequest): Observable<ArquivoResponse> {
    return this.http.post<ArquivoResponse>(`${this.despesasBase}/arquivo`, request);
  }

  registrarArquivo(despesaId: string, file: File): Observable<ArquivoResponse> {
    return from(buildArquivoRequest(file, despesaId)).pipe(
      switchMap((request) => this.registrar(request)),
    );
  }

  obterBlob(arquivoId: string): Observable<Blob> {
    return this.http.get(`${this.arquivosBase}/${arquivoId}`, { responseType: 'blob' });
  }

  visualizar(arquivoId: string): Observable<void> {
    return this.obterBlob(arquivoId).pipe(
      map((blob) => {
        const url = URL.createObjectURL(blob);
        window.open(url, '_blank', 'noopener');
        setTimeout(() => URL.revokeObjectURL(url), 60_000);
      }),
    );
  }
}