import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Account, CreateAccountDto, UpdateAccountDto } from '../models/models';

@Injectable({ providedIn: 'root' })
export class AccountService {
  private readonly baseUrl = 'http://localhost:5000/api/accounts';

  constructor(private http: HttpClient) {}

  getAll(): Observable<Account[]> {
    return this.http.get<Account[]>(this.baseUrl);
  }

  getById(id: string): Observable<Account> {
    return this.http.get<Account>(`${this.baseUrl}/${id}`);
  }

  create(dto: CreateAccountDto): Observable<Account> {
    return this.http.post<Account>(this.baseUrl, dto);
  }

  update(id: string, dto: UpdateAccountDto): Observable<Account> {
    return this.http.put<Account>(`${this.baseUrl}/${id}`, dto);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
