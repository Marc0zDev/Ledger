import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthService } from './auth.service';
import { CriarReceitaRequest, ReceitaResponse } from '../models/receita.model';

@Injectable({ providedIn: 'root' })
export class ReceitaService {
  private readonly http = inject(HttpClient);
  private readonly auth = inject(AuthService);
  private readonly base = `${environment.apiUrl}/api/receita`;

  listar(): Observable<ReceitaResponse[]> {
    const usuarioId = this.auth.currentUser()!.usuarioId;
    return this.http.get<ReceitaResponse[]>(`${this.base}/${usuarioId}`);
  }

  criar(request: CriarReceitaRequest): Observable<{ id: string }> {
    return this.http.post<{ id: string }>(this.base, request);
  }
}
