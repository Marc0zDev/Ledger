import { Component, input, output } from '@angular/core';
import { NgTemplateOutlet } from '@angular/common';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { ButtonModule } from 'primeng/button';
import { LedgerColumn, LedgerAction, RowActionEvent } from './ledger-table.types';

export type { LedgerColumn, LedgerAction, RowActionEvent } from './ledger-table.types';

@Component({
  selector: 'app-ledger-table',
  standalone: true,
  imports: [TableModule, TagModule, ButtonModule, NgTemplateOutlet],
  templateUrl: './ledger-table.component.html',
  styleUrl: './ledger-table.component.scss',
})
export class LedgerTableComponent {
  dataSource   = input<unknown[]>([]);
  options      = input<LedgerColumn[]>([]);
  loading      = input<boolean>(false);
  rowHover     = input<boolean>(true);

  /** Tamanho da página. 0 = sem paginação */
  rows         = input<number>(0);
  totalRecords = input<number>(0);
  /** Paginação server-side */
  lazy         = input<boolean>(false);

  rowAction = output<RowActionEvent>();
  lazyLoad  = output<unknown>();

  getValue(row: unknown, field?: string): unknown {
    if (!field || typeof row !== 'object' || row === null) return null;
    return (row as Record<string, unknown>)[field];
  }

  getTagLabel(col: LedgerColumn, row: unknown): string {
    const value = this.getValue(row, col.field);
    return col.tagLabel ? col.tagLabel(value, row) : String(value ?? '');
  }

  getTagSeverity(col: LedgerColumn, row: unknown): 'success' | 'secondary' | 'info' | 'warn' | 'danger' | 'contrast' {
    const value = this.getValue(row, col.field);
    const s = col.tagSeverity ? col.tagSeverity(value, row) : 'secondary';
    return s as 'success' | 'secondary' | 'info' | 'warn' | 'danger' | 'contrast';
  }

  formatCurrency(value: unknown): string {
    if (typeof value !== 'number') return String(value ?? '');
    return value.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
  }

  formatDate(value: unknown): string {
    if (!value) return '';
    return new Date(String(value)).toLocaleDateString('pt-BR');
  }

  isVisible(action: LedgerAction, row: unknown): boolean {
    return action.visible ? action.visible(row) : true;
  }

  isDisabled(action: LedgerAction, row: unknown): boolean {
    return action.disabled ? action.disabled(row) : false;
  }

  onAction(event: string, row: unknown): void {
    this.rowAction.emit({ event, row });
  }

  onFileChange(domEvent: Event, actionEvent: string, row: unknown): void {
    const file = (domEvent.target as HTMLInputElement).files?.[0];
    if (file) this.rowAction.emit({ event: actionEvent, row, file });
  }
}
