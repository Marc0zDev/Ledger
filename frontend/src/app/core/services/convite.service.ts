import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ParticipanteResponse } from '../models/cofre.model';

export interface ConviteResponse {
  id: string;
  cofreId: string;
  cofreNome: string;
  token: string;
  status: string;
  expiresAt: string;
  createdAt: string;
}

@Injectable({ providedIn: 'root' })
export class ConviteService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/api/convites`;

  pendentes(): Observable<ConviteResponse[]> {
    return this.http.get<ConviteResponse[]>(`${this.base}/pendentes`);
  }

  aceitar(token: string): Observable<ParticipanteResponse> {
    return this.http.post<ParticipanteResponse>(`${this.base}/${token}/aceitar`, {});
  }

  recusar(token: string): Observable<ConviteResponse> {
    return this.http.post<ConviteResponse>(`${this.base}/${token}/recusar`, {});
  }
}
