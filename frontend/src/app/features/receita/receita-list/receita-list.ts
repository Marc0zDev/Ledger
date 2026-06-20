import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ReceitaService } from '../../../core/services/receita.service';
import { AuthService } from '../../../core/services/auth.service';
import { GrupoService } from '../../../core/services/grupo.service';
import { NotificationService } from '../../../core/services/notification.service';
import { ReceitaResponse, ReceitaTemplateResponse } from '../../../core/models/receita.model';
import { GrupoResponse } from '../../../core/models/grupo.model';
import { LedgerTableComponent, LedgerColumn, RowActionEvent } from '../../../shared/components/ledger-table/ledger-table.component';

function primeiroDiaMes(d: Date): Date {
  return new Date(Date.UTC(d.getUTCFullYear(), d.getUTCMonth(), 1));
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
  private readonly grupoService    = inject(GrupoService);
  private readonly fb             = inject(FormBuilder);

  grupos = signal<GrupoResponse[]>([]);

  // ── Abas ─────────────────────────────────────────────────────────────────
  activeTab = signal<'receitas' | 'fixas'>('receitas');

  // ── Estado receitas ───────────────────────────────────────────────────────
  receitas    = signal<ReceitaResponse[]>([]);
  competencia = signal<Date>(primeiroDiaMes(new Date()));
  loading     = signal(false);
  showForm    = signal(false);
  saving      = signal(false);
  gerando     = signal(false);

  // ── Estado templates ──────────────────────────────────────────────────────
  templates         = signal<ReceitaTemplateResponse[]>([]);
  loadingTemplates  = signal(false);
  showTemplateForm  = signal(false);
  savingTemplate    = signal(false);
  editingTemplateId = signal<string | null>(null);

  // ── Computed ─────────────────────────────────────────────────────────────
  competenciaLabel = computed(() =>
    this.competencia().toLocaleString('pt-BR', { month: 'long', year: 'numeric', timeZone: 'UTC' })
  );

  totalMes = computed(() => this.receitas().reduce((acc, r) => acc + r.valor, 0));

  // ── Forms ─────────────────────────────────────────────────────────────────
  form = this.fb.group({
    nome:            ['', [Validators.required, Validators.maxLength(100)]],
    valor:           [0,  [Validators.required, Validators.min(0.01)]],
    descricao:       [''],
    dataRecebimento: ['', Validators.required],
    grupoId:         [null as string | null],
  });

  templateForm = this.fb.nonNullable.group({
    nome:       ['', [Validators.required, Validators.maxLength(100)]],
    valor:      [0,  [Validators.required, Validators.min(0.01)]],
    descricao:  [''],
    dataInicio: ['', Validators.required],
    dataFim:    [''],
  });

  // ── Colunas ───────────────────────────────────────────────────────────────
  readonly colunasReceitas: LedgerColumn[] = [
    { field: 'nome',            title: 'Nome' },
    { field: 'valor',           title: 'Valor',    type: 'currency' },
    { field: 'dataRecebimento', title: 'Recebido', type: 'date' },
    { field: 'descricao',       title: 'Descrição' },
    {
      field: 'id', title: '', type: 'actions',
      actions: [{ icon: 'pi-trash', severity: 'danger', event: 'deletar', label: 'Excluir' }],
    },
  ];

  readonly colunasTemplates: LedgerColumn[] = [
    { field: 'nome',       title: 'Nome' },
    { field: 'valor',      title: 'Valor',  type: 'currency' },
    { field: 'dataInicio', title: 'Início', type: 'date' },
    { field: 'dataFim',    title: 'Fim',    type: 'date' },
    { field: 'descricao',  title: 'Descrição' },
    {
      field: 'id', title: '', type: 'actions',
      actions: [
        { icon: 'pi-pencil', event: 'editar',  label: 'Editar' },
        { icon: 'pi-trash',  severity: 'danger', event: 'deletar', label: 'Excluir' },
      ],
    },
  ];

  ngOnInit(): void {
    this.grupoService.listar().subscribe({ next: (g) => this.grupos.set(g) });
    this.carregarReceitas();
    this.carregarTemplates();
  }

  // ── Abas ─────────────────────────────────────────────────────────────────
  mudarAba(tab: 'receitas' | 'fixas'): void {
    this.activeTab.set(tab);
  }

  // ── Receitas ──────────────────────────────────────────────────────────────
  carregarReceitas(): void {
    this.loading.set(true);
    this.receitaService.listar(this.competencia()).subscribe({
      next:  (data) => { this.receitas.set(data); this.loading.set(false); },
      error: ()     => { this.notify.error('Erro ao carregar receitas.'); this.loading.set(false); },
    });
  }

  mesAnterior(): void {
    const d = this.competencia();
    this.competencia.set(primeiroDiaMes(new Date(Date.UTC(d.getUTCFullYear(), d.getUTCMonth() - 1, 1))));
    this.carregarReceitas();
  }

  mesSeguinte(): void {
    const d = this.competencia();
    this.competencia.set(primeiroDiaMes(new Date(Date.UTC(d.getUTCFullYear(), d.getUTCMonth() + 1, 1))));
    this.carregarReceitas();
  }

  abrirForm(): void {
    this.form.reset({ dataRecebimento: new Date().toISOString().substring(0, 10), grupoId: null });
    this.showForm.set(true);
  }

  fecharForm(): void {
    this.showForm.set(false);
    this.form.reset();
  }

  salvar(): void {
    if (this.form.invalid || this.saving()) return;
    this.saving.set(true);
    const { nome, valor, descricao, dataRecebimento, grupoId } = this.form.getRawValue();
    const usuarioId = this.auth.currentUser()!.usuarioId;

    this.receitaService.criar({ usuarioId, nome: nome!, valor: valor!, descricao: descricao ?? undefined, dataRecebimento: dataRecebimento!, grupoId: grupoId ?? undefined }).subscribe({
      next: () => {
        this.notify.success('Receita registrada com sucesso.');
        this.fecharForm();
        this.carregarReceitas();
        this.saving.set(false);
      },
      error: () => {
        this.notify.error('Erro ao registrar receita.');
        this.saving.set(false);
      },
    });
  }

  gerarDoMes(): void {
    if (this.gerando()) return;
    this.gerando.set(true);
    this.receitaService.gerarDoMes(this.competencia()).subscribe({
      next: (criadas) => {
        if (criadas.length === 0) {
          this.notify.info('Todas as receitas fixas já foram geradas para este mês.');
        } else {
          this.notify.success(`${criadas.length} receita(s) gerada(s) com sucesso.`);
          this.carregarReceitas();
        }
        this.gerando.set(false);
      },
      error: () => {
        this.notify.error('Erro ao gerar receitas do mês.');
        this.gerando.set(false);
      },
    });
  }

  onActionReceita(event: RowActionEvent): void {
    const r = event.row as ReceitaResponse;
    if (event.event === 'deletar') {
      this.receitaService.deletar(r.id).subscribe({
        next: () => {
          this.notify.success('Receita excluída.');
          this.carregarReceitas();
        },
        error: () => this.notify.error('Erro ao excluir receita.'),
      });
    }
  }

  // ── Templates ─────────────────────────────────────────────────────────────
  carregarTemplates(): void {
    this.loadingTemplates.set(true);
    this.receitaService.listarTemplates().subscribe({
      next:  (data) => { this.templates.set(data); this.loadingTemplates.set(false); },
      error: ()     => { this.notify.error('Erro ao carregar receitas fixas.'); this.loadingTemplates.set(false); },
    });
  }

  abrirTemplateForm(template?: ReceitaTemplateResponse): void {
    if (template) {
      this.editingTemplateId.set(template.id);
      this.templateForm.setValue({
        nome:       template.nome,
        valor:      template.valor,
        descricao:  template.descricao ?? '',
        dataInicio: template.dataInicio.substring(0, 10),
        dataFim:    template.dataFim ? template.dataFim.substring(0, 10) : '',
      });
    } else {
      this.editingTemplateId.set(null);
      this.templateForm.reset({ dataInicio: new Date().toISOString().substring(0, 10) });
    }
    this.showTemplateForm.set(true);
  }

  fecharTemplateForm(): void {
    this.showTemplateForm.set(false);
    this.templateForm.reset();
    this.editingTemplateId.set(null);
  }

  salvarTemplate(): void {
    if (this.templateForm.invalid || this.savingTemplate()) return;
    this.savingTemplate.set(true);
    const { nome, valor, descricao, dataInicio, dataFim } = this.templateForm.getRawValue();
    const editId = this.editingTemplateId();
    const usuarioId = this.auth.currentUser()!.usuarioId;

    const req$ = editId
      ? this.receitaService.atualizarTemplate(editId, { nome, valor, descricao, dataInicio, dataFim: dataFim || undefined })
      : this.receitaService.criarTemplate({ usuarioId, nome, valor, descricao, dataInicio, dataFim: dataFim || undefined });

    req$.subscribe({
      next: () => {
        this.notify.success(editId ? 'Receita fixa atualizada.' : 'Receita fixa criada.');
        this.fecharTemplateForm();
        this.carregarTemplates();
        this.savingTemplate.set(false);
      },
      error: () => {
        this.notify.error('Erro ao salvar receita fixa.');
        this.savingTemplate.set(false);
      },
    });
  }

  onActionTemplate(event: RowActionEvent): void {
    const t = event.row as ReceitaTemplateResponse;
    if (event.event === 'editar') {
      this.abrirTemplateForm(t);
      this.mudarAba('fixas');
    }
    if (event.event === 'deletar') {
      this.receitaService.deletarTemplate(t.id).subscribe({
        next: () => {
          this.notify.success('Receita fixa excluída.');
          this.carregarTemplates();
        },
        error: () => this.notify.error('Erro ao excluir receita fixa.'),
      });
    }
  }

  formatarMoeda(v: number): string {
    return v.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
  }
}
