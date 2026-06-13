import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ReceitaService } from '../../../core/services/receita.service';
import { AuthService } from '../../../core/services/auth.service';
import { NotificationService } from '../../../core/services/notification.service';
import { ReceitaResponse } from '../../../core/models/receita.model';
import { LedgerTableComponent, LedgerColumn, RowActionEvent } from '../../../shared/components/ledger-table/ledger-table.component';

function primeiroDiaMes(d: Date): string {
  return new Date(Date.UTC(d.getFullYear(), d.getMonth(), 1)).toISOString().substring(0, 10);
}

@Component({
  selector: 'app-receita-list',
  standalone: true,
  imports: [ReactiveFormsModule, LedgerTableComponent],
  templateUrl: './receita-list.html',
  styleUrl: './receita-list.scss',
})
export class ReceitaList implements OnInit {
  private readonly receitaService = inject(ReceitaService);
  private readonly auth           = inject(AuthService);
  private readonly notify         = inject(NotificationService);
  private readonly fb             = inject(FormBuilder);

  // ── Estado ───────────────────────────────────────────────────────────────
  receitas    = signal<ReceitaResponse[]>([]);
  competencia = signal<string>(primeiroDiaMes(new Date()));
  loading     = signal(true);
  showForm    = signal(false);
  saving      = signal(false);

  // ── Computed ─────────────────────────────────────────────────────────────
  competenciaLabel = computed(() => {
    const [ano, mes] = this.competencia().split('-');
    return new Date(+ano, +mes - 1, 1).toLocaleString('pt-BR', { month: 'long', year: 'numeric' });
  });

  receitasMes = computed(() => {
    const [ano, mes] = this.competencia().split('-').map(Number);
    return this.receitas().filter(r => {
      const d = new Date(r.dataRecebimento);
      return d.getUTCFullYear() === ano && d.getUTCMonth() + 1 === mes;
    });
  });

  totalMes = computed(() =>
    this.receitasMes().reduce((acc, r) => acc + r.valor, 0)
  );

  // ── Form ─────────────────────────────────────────────────────────────────
  form = this.fb.nonNullable.group({
    nome:             ['', [Validators.required, Validators.maxLength(100)]],
    valor:            [0,  [Validators.required, Validators.min(0.01)]],
    descricao:        [''],
    dataRecebimento:  ['', Validators.required],
  });

  // ── Tabela ────────────────────────────────────────────────────────────────
  readonly colunas: LedgerColumn[] = [
    { field: 'nome',            title: 'Nome' },
    { field: 'valor',           title: 'Valor',            type: 'currency' },
    { field: 'dataRecebimento', title: 'Data recebimento', type: 'date' },
    { field: 'descricao',       title: 'Descrição' },
    {
      field: 'id',
      title: '',
      type: 'actions',
      actions: [
        { icon: 'pi-trash', severity: 'danger', event: 'deletar', label: 'Excluir' },
      ],
    },
  ];

  ngOnInit(): void {
    this.carregar();
  }

  private carregar(): void {
    this.loading.set(true);
    this.receitaService.listar().subscribe({
      next: (data) => { this.receitas.set(data); this.loading.set(false); },
      error: () => { this.notify.error('Erro ao carregar receitas.'); this.loading.set(false); },
    });
  }

  // ── Navegação de mês ──────────────────────────────────────────────────────
  mesAnterior(): void {
    const [ano, mes] = this.competencia().split('-').map(Number);
    const d = new Date(Date.UTC(ano, mes - 2, 1));
    this.competencia.set(d.toISOString().substring(0, 10));
  }

  mesSeguinte(): void {
    const [ano, mes] = this.competencia().split('-').map(Number);
    const d = new Date(Date.UTC(ano, mes, 1));
    this.competencia.set(d.toISOString().substring(0, 10));
  }

  // ── Form ─────────────────────────────────────────────────────────────────
  abrirForm(): void {
    this.form.reset({ dataRecebimento: new Date().toISOString().substring(0, 10) });
    this.showForm.set(true);
  }

  fecharForm(): void {
    this.showForm.set(false);
    this.form.reset();
  }

  salvar(): void {
    if (this.form.invalid || this.saving()) return;
    this.saving.set(true);
    const { nome, valor, descricao, dataRecebimento } = this.form.getRawValue();
    const usuarioId = this.auth.currentUser()!.usuarioId;

    this.receitaService.criar({ usuarioId, nome, valor, descricao, dataRecebimento }).subscribe({
      next: () => {
        this.notify.success('Receita registrada com sucesso.');
        this.fecharForm();
        this.carregar();
        this.saving.set(false);
      },
      error: () => {
        this.notify.error('Erro ao registrar receita.');
        this.saving.set(false);
      },
    });
  }

  // ── Ações da tabela ───────────────────────────────────────────────────────
  onAction(event: RowActionEvent): void {
    const r = event.row as ReceitaResponse;
    if (event.event === 'deletar') {
      this.notify.warn('Exclusão ainda não implementada no servidor.');
    }
  }

  // ── Helpers ───────────────────────────────────────────────────────────────
  formatarMoeda(v: number): string {
    return v.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
  }
}
