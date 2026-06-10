import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { tap } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface AuthResponse {
  token: string;
  nome: string;
  email: string;
  usuarioId: string;
  expiresAt: string;
}

export interface LoginRequest {
  email: string;
  senha: string;
}

export interface RegistrarRequest {
  nome: string;
  email: string;
  senha: string;
}

const TOKEN_KEY = 'ledger_token';
const USER_KEY  = 'ledger_user';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http   = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly base   = `${environment.apiUrl}/api/auth`;

  currentUser = signal<AuthResponse | null>(this.loadUser());

  private loadUser(): AuthResponse | null {
    try {
      const raw = localStorage.getItem(USER_KEY);
      return raw ? JSON.parse(raw) : null;
    } catch {
      return null;
    }
  }

  isAuthenticated(): boolean {
    const user = this.currentUser();
    if (!user) return false;
    return new Date(user.expiresAt) > new Date();
  }

  getToken(): string | null {
    return this.currentUser()?.token ?? null;
  }

  login(request: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.base}/login`, request).pipe(
      tap((res) => this.persistSession(res))
    );
  }

  registrar(request: RegistrarRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.base}/registrar`, request).pipe(
      tap((res) =>
        this.router.navigate(['/aguardando-confirmacao'], {
          queryParams: { userId: res.usuarioId, email: res.email },
        })
      )
    );
  }

  logout(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    this.currentUser.set(null);
    this.router.navigate(['/login']);
  }

  private persistSession(res: AuthResponse): void {
    localStorage.setItem(USER_KEY, JSON.stringify(res));
    this.currentUser.set(res);
    this.router.navigate(['/home']);
  }
}
