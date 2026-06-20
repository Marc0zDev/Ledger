import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CategoriaResponse } from '../models/cofre.model';

@Injectable({ providedIn: 'root' })
export class CategoriaService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/api/categorias`;

  listar(): Observable<CategoriaResponse[]> {
    return this.http.get<CategoriaResponse[]>(this.base);
  }
}
