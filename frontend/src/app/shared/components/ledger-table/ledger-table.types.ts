import { TemplateRef } from '@angular/core';

export type LedgerColumnType = 'text' | 'currency' | 'tag' | 'date' | 'actions' | 'custom';

export interface LedgerAction {
  /** PrimeIcons class sem o prefixo 'pi', ex: 'pi-pencil' */
  icon: string;
  severity?: 'primary' | 'secondary' | 'success' | 'info' | 'warn' | 'danger' | 'contrast';
  /** Nome do evento emitido em (rowAction) */
  event: string;
  /** Texto exibido no menu. Fallback: title → event */
  label?: string;
  title?: string;
  disabled?: (row: unknown) => boolean;
  visible?: (row: unknown) => boolean;
  /** Torna o botão um input de arquivo */
  isFileUpload?: boolean;
  accept?: string;
}

export interface LedgerColumn {
  title: string;
  /** Nome da propriedade no objeto de dados */
  field?: string;
  type?: LedgerColumnType;
  width?: string;
  align?: 'left' | 'center' | 'right';
  /** Para type='tag': retorna o label do badge */
  tagLabel?: (value: unknown, row: unknown) => string;
  /** Para type='tag': retorna a severidade do badge */
  tagSeverity?: (value: unknown, row: unknown) => string;
  /** Para type='actions': lista de ações no menu kebab */
  actions?: LedgerAction[];
  /** Controla o que aparece nos itens do menu. Default: 'icon-label' */
  actionMenuMode?: 'icon-label' | 'label-only';
  /** Para type='custom': template livre. Contexto: { $implicit: row, value: row[field] } */
  cellTpl?: TemplateRef<{ $implicit: unknown; value: unknown }>;
}

export interface RowActionEvent {
  event: string;
  row: unknown;
  file?: File;
}
