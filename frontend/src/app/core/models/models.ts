export enum TransactionType {
  Income = 1,
  Expense = 2,
  Transfer = 3,
}

export interface Account {
  id: string;
  name: string;
  description?: string;
  initialBalance: number;
  currency: string;
  createdAt: string;
}

export interface Transaction {
  id: string;
  description: string;
  amount: number;
  type: TransactionType;
  date: string;
  accountId: string;
  category?: string;
  notes?: string;
  createdAt: string;
}

export interface CreateAccountDto {
  name: string;
  initialBalance: number;
  currency: string;
  description?: string;
}

export interface UpdateAccountDto {
  name: string;
  currency: string;
  description?: string;
}

export interface CreateTransactionDto {
  description: string;
  amount: number;
  type: TransactionType;
  date: string;
  accountId: string;
  category?: string;
  notes?: string;
}

export interface UpdateTransactionDto {
  description: string;
  amount: number;
  type: TransactionType;
  date: string;
  category?: string;
  notes?: string;
}
