import { Injectable } from '@angular/core';
import { MessageService } from 'primeng/api';

@Injectable({ providedIn: 'root' })
export class NotificationService {
  constructor(private readonly messages: MessageService) {}

  success(detail: string, summary = 'Sucesso'): void {
    this.messages.add({ severity: 'success', summary, detail, life: 4000 });
  }

  error(detail: string, summary = 'Erro'): void {
    this.messages.add({ severity: 'error', summary, detail, life: 6000 });
  }

  warn(detail: string, summary = 'Atenção'): void {
    this.messages.add({ severity: 'warn', summary, detail, life: 5000 });
  }

  info(detail: string, summary = 'Info'): void {
    this.messages.add({ severity: 'info', summary, detail, life: 4000 });
  }
}
