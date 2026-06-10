import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Transaction, CreateTransactionDto, UpdateTransactionDto } from '../models/models';

@Injectable({ providedIn: 'root' })
export class TransactionService {
  private readonly baseUrl = 'http://localhost:5000/api/transactions';

  constructor(private http: HttpClient) {}

  getAll(): Observable<Transaction[]> {
    return this.http.get<Transaction[]>(this.baseUrl);
  }

  getById(id: string): Observable<Transaction> {
    return this.http.get<Transaction>(`${this.baseUrl}/${id}`);
  }

  getByAccount(accountId: string): Observable<Transaction[]> {
    return this.http.get<Transaction[]>(`${this.baseUrl}/account/${accountId}`);
  }

  create(dto: CreateTransactionDto): Observable<Transaction> {
    return this.http.post<Transaction>(this.baseUrl, dto);
  }

  update(id: string, dto: UpdateTransactionDto): Observable<Transaction> {
    return this.http.put<Transaction>(`${this.baseUrl}/${id}`, dto);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
