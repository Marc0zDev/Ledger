import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { UsuarioResponse, CriarUsuarioRequest, AtualizarUsuarioRequest } from '../models/cofre.model';

@Injectable({ providedIn: 'root' })
export class UsuarioService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/api/usuarios`;

  listar(): Observable<UsuarioResponse[]> {
    return this.http.get<UsuarioResponse[]>(this.base);
  }

  obterPorId(id: string): Observable<UsuarioResponse> {
    return this.http.get<UsuarioResponse>(`${this.base}/${id}`);
  }

  obterPorEmail(email: string): Observable<UsuarioResponse> {
    return this.http.get<UsuarioResponse>(`${this.base}/email/${email}`);
  }

  criar(request: CriarUsuarioRequest): Observable<UsuarioResponse> {
    return this.http.post<UsuarioResponse>(this.base, request);
  }

  atualizar(id: string, request: AtualizarUsuarioRequest): Observable<UsuarioResponse> {
    return this.http.put<UsuarioResponse>(`${this.base}/${id}`, request);
  }

  deletar(id: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`);
  }
}
