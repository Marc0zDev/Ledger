import {
  Component, inject, OnInit, signal, computed
} from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { DespesaService } from '../../../core/services/despesa.service';
import { DespesaPeriodoService } from '../../../core/services/despesa-periodo.service';
import { ArquivoService } from '../../../core/services/arquivo.service';
import { CategoriaService } from '../../../core/services/categoria.service';
import { NotificationService } from '../../../core/services/notification.service';
import { FileViewerService } from '../../../core/services/file-viewer.service';
import { ArquivoResponse } from '../../../core/models/arquivo.model';
import {
  DespesaResponse,
  DespesaPeriodoResponse,
  CategoriaResponse,
  TipoDespesa,
} from '../../../core/models/cofre.model';
import { environment } from '../../../../environments/environment';
import { LedgerTableComponent, LedgerColumn, RowActionEvent } from '../../../shared/components/ledger-table/ledger-table.component';
import { TagModule } from 'primeng/tag';
import { ButtonModule } from 'primeng/button';

type Aba = 'periodo' | 'templates';

function primeiroDiaMes(d: Date): string {
  return new Date(Date.UTC(d.getFullYear(), d.getMonth(), 1)).toISOString().substring(0, 10);
}

@Component({
  selector: 'app-despesas',
  standalone: true,
  imports: [ReactiveFormsModule, LedgerTableComponent, TagModule, ButtonModule],
  templateUrl: './despesas.component.html',
  styleUrl: './despesas.component.scss',
})
export class DespesasComponent implements OnInit {
  private readonly despesaService       = inject(DespesaService);
  private readonly periodoService       = inject(DespesaPeriodoService);
  private readonly arquivoService       = inject(ArquivoService);
  private readonly categoriaService     = inject(CategoriaService);
  private readonly notify               = inject(NotificationService);
  private readonly fileViewer           = inject(FileViewerService);
  private readonly fb                   = inject(FormBuilder);

  readonly TIPOS: { valor: TipoDespesa; label: string }[] = [
    { valor: 1, label: 'Fixa' },
    { valor: 2, label: 'Variável' },
    { valor: 3, label: 'Avulsa' },
  ];

  // ── Estado ───────────────────────────────────────────────────────────────
  categorias    = signal<CategoriaResponse[]>([]);
  templates     = signal<DespesaResponse[]>([]);
  periodos      = signal<DespesaPeriodoResponse[]>([]);
  competencia   = signal<string>(primeiroDiaMes(new Date()));
  loading       = signal(true);
  abaAtiva      = signal<Aba>('periodo');
  showForm      = signal(false);
  saving        = signal(false);
  editandoPeriodo  = signal<DespesaPeriodoResponse | null>(null);
  editandoTemplate = signal<DespesaResponse | null>(null);
  pagandoId     = signal<string | null>(null);
  deletandoId   = signal<string | null>(null);
  uploadingArquivoId  = signal<string | null>(null);
  arquivosAnexados    = signal<Record<string, ArquivoResponse>>({});
  gerando       = signal(false);

  // Paginação de contas fixas
  readonly tmPageSize = 10;
  tmPage      = signal(1);
  tmTotal     = signal(0);
  tmTotalPags = signal(1);
  tmLoading   = signal(false);
  tmPodeAnterior = computed(() => this.tmPage() > 1);
  tmPodeProxima  = computed(() => this.tmPage() < this.tmTotalPags());

  // ── Computed ─────────────────────────────────────────────────────────────
  competenciaLabel = computed(() => {
    const [ano, mes] = this.competencia().split('-');
    return new Date(+ano, +mes - 1, 1).toLocaleString('pt-BR', { month: 'long', year: 'numeric' });
  });

  totalPlanejado = computed(() =>
    this.periodos().reduce((s, d) => s + d.valorPlanejado, 0));

  totalRealizado = computed(() =>
    this.periodos().filter(d => d.paga).reduce((s, d) => s + d.valorRealizado, 0));

  pendentesCount = computed(() => this.periodos().filter(d => !d.paga).length);

  // ── Colunas da tabela ────────────────────────────────────────────────────
  periodoColumns: LedgerColumn[] = [
    { title: 'Descrição', field: 'descricao', width: '20' },
    { title: 'Categoria', field: 'categoriaNome', width: '20' },
    { title: 'Valor', field: 'valorPlanejado', type: 'currency', width: '20%', align: 'right' },
    {
      title: 'Status', type: 'tag', width: '20%',
      tagLabel:    (_, row) => this.statusLabel(row as DespesaPeriodoResponse),
      tagSeverity: (_, row) => this.statusSeverity(row as DespesaPeriodoResponse),
    },
    {
      title: '', type: 'actions', width: '20%',
      actions: [
        {
          icon: 'pi-check', severity: 'success', event: 'pagar', label: 'Marcar como pago',
          visible:  (r) => !(r as DespesaPeriodoResponse).paga,
          disabled: (r) => this.pagandoId() === (r as DespesaPeriodoResponse).id,
        },
        {
          icon: 'pi-file-pdf', severity: 'secondary', event: 'verBoleto', label: 'Ver boleto',
          visible: (r) => !!(r as DespesaPeriodoResponse).boletoUrl,
        },
        {
          icon: 'pi-upload', severity: 'secondary', event: 'uploadArquivo', label: 'Anexar arquivo',
          isFileUpload: true,
          visible:  (r) => !this.periodoTemArquivo(r as DespesaPeriodoResponse) && !(r as DespesaPeriodoResponse).paga,
          disabled: (r) => this.uploadingArquivoId() === (r as DespesaPeriodoResponse).id,
        },
        {
          icon: 'pi-eye', severity: 'info', event: 'verArquivoPeriodo', label: 'Visualizar arquivo',
          visible: (r) => this.periodoTemArquivo(r as DespesaPeriodoResponse),
        },
        {
          icon: 'pi-receipt', severity: 'secondary', event: 'uploadComprovante', label: 'Anexar comprovante',
          isFileUpload: true,
          visible:  (r) => !!(r as DespesaPeriodoResponse).paga && !(r as DespesaPeriodoResponse).comprovanteId,
          disabled: (r) => this.uploadingArquivoId() === (r as DespesaPeriodoResponse).id + '-comprovante',
        },
        {
          icon: 'pi-receipt', severity: 'info', event: 'verComprovante', label: 'Ver comprovante',
          visible:  (r) => !!(r as DespesaPeriodoResponse).paga,
          disabled: (r) => !(r as DespesaPeriodoResponse).comprovanteId,
        },
        { icon: 'pi-pencil', severity: 'secondary', event: 'editar', label: 'Editar' },
        {
          icon: 'pi-trash', severity: 'danger', event: 'deletar', label: 'Excluir',
          disabled: (r) => this.deletandoId() === (r as DespesaPeriodoResponse).id,
        },
      ],
    },
  ];

  cfColumns: LedgerColumn[] = [
    { title: 'Nome',       field: 'nome',           width: '28%' },
    { title: 'Categoria',  field: 'categoriaNome',   width: '20%' },
    {
      title: 'Tipo', field: 'tipo', type: 'tag', width: '10%',
      tagSeverity: (v) => this.tipoSeverity(v as string),
    },
    { title: 'Vencimento', field: 'diaVencimento',   width: '11%',
      tagLabel: (v) => v ? `dia ${v}` : '—' },
    { title: 'Valor', field: 'valorPlanejado', type: 'currency', width: '14%', align: 'right' },
    {
      title: 'Status', field: 'ativa', type: 'tag', width: '10%',
      tagLabel:    (v) => v ? 'Ativo' : 'Inativo',
      tagSeverity: (v) => v ? 'success' : 'secondary',
    },
    {
      title: '', type: 'actions', width: '12%',
      actions: [
        {
          icon: 'pi-upload', severity: 'secondary', event: 'uploadArquivo', label: 'Anexar arquivo',
          isFileUpload: true,
          visible:  (r) => !this.templateTemArquivo(r as DespesaResponse),
          disabled: (r) => this.uploadingArquivoId() === (r as DespesaResponse).id,
        },
        {
          icon: 'pi-eye', severity: 'info', event: 'verArquivo', label: 'Visualizar arquivo',
          visible: (r) => this.templateTemArquivo(r as DespesaResponse),
        },
        { icon: 'pi-pencil', severity: 'secondary', event: 'editar',    label: 'Editar' },
        { icon: 'pi-pause',  severity: 'secondary', event: 'desativar', label: 'Desativar', visible: (r) => !!(r as DespesaResponse).ativa },
        {
          icon: 'pi-trash',  severity: 'danger',    event: 'deletar',  label: 'Excluir',
          disabled: (r) => this.deletandoId() === (r as DespesaResponse).id,
        },
      ],
    },
  ];

  // ── Row action handler ────────────────────────────────────────────────────
  onPeriodoAction({ event, row, file }: RowActionEvent): void {
    const d = row as DespesaPeriodoResponse;
    switch (event) {
      case 'pagar':       this.pagar(d); break;
      case 'verBoleto':   window.open(this.boletoUrl(d.boletoUrl!), '_blank', 'noopener'); break;
      case 'uploadArquivo': if (file) this.anexarArquivoPeriodo(d, file); break;
      case 'verArquivoPeriodo': this.verArquivoPeriodo(d); break;
      case 'uploadComprovante': if (file) this.anexarComprovante(d, file); break;
      case 'verComprovante': if (d.comprovanteId) this.abrirArquivo(d.comprovanteId); break;
      case 'editar':      this.abrirFormPeriodo(d); break;
      case 'deletar':     this.deletarPeriodo(d.id); break;
    }
  }

  onCfAction({ event, row, file }: RowActionEvent): void {
    const t = row as DespesaResponse;
    switch (event) {
      case 'uploadArquivo': if (file) this.anexarArquivoTemplate(t, file); break;
      case 'verArquivo':    this.verArquivo(t); break;
      case 'editar':    this.abrirFormTemplate(t); break;
      case 'desativar': this.desativarTemplate(t.id); break;
      case 'deletar':   this.deletarTemplate(t.id); break;
    }
  }

  // ── Forms ─────────────────────────────────────────────────────────────────
  formPeriodo = this.fb.group({
    descricao:      ['', [Validators.required, Validators.maxLength(200)]],
    valorPlanejado: [null as number | null, [Validators.required, Validators.min(0.01)]],
    categoriaId:    ['', Validators.required],
    despesaId:      [null as string | null],
  });

  formTemplate = this.fb.group({
    nome:           ['', [Validators.required, Validators.maxLength(200)]],
    tipo:           [3 as TipoDespesa, Validators.required],
    valorPlanejado: [null as number | null, [Validators.required, Validators.min(0.01)]],
    categoriaId:    ['', Validators.required],
    diaVencimento:  [null as number | null, [Validators.min(1), Validators.max(31)]],
  });

  // ── Lifecycle ─────────────────────────────────────────────────────────────
  ngOnInit(): void {
    this.categoriaService.listar().subscribe({
      next: (cats) => {
        this.categorias.set(cats);
        if (cats.length > 0 && !this.formPeriodo.value.categoriaId) {
          this.formPeriodo.patchValue({ categoriaId: cats[0].id });
          this.formTemplate.patchValue({ categoriaId: cats[0].id });
        }
      },
    });
    this.carregarPeriodo();
    this.carregarTemplates();
  }

  carregarPeriodo(): void {
    this.loading.set(true);
    this.periodoService.listarPorCompetencia(this.competencia()).subscribe({
      next: (list) => { this.periodos.set(list); this.loading.set(false); },
      error: () => this.loading.set(false),
    });
  }

  carregarTemplates(page = 1): void {
    this.tmLoading.set(true);
    this.despesaService.listar(page, this.tmPageSize).subscribe({
      next: (r) => {
        this.templates.set(r.items ?? []);
        this.tmPage.set(r.page ?? 1);
        this.tmTotal.set(r.total ?? 0);
        this.tmTotalPags.set(r.totalPages ?? 1);
        this.tmLoading.set(false);
      },
      error: () => this.tmLoading.set(false),
    });
  }

  tmAnterior(): void { if (this.tmPodeAnterior()) this.carregarTemplates(this.tmPage() - 1); }
  tmProxima():  void { if (this.tmPodeProxima())  this.carregarTemplates(this.tmPage() + 1); }

  // ── Navegação de mês ──────────────────────────────────────────────────────
  mesAnterior(): void {
    const [ano, mes] = this.competencia().split('-').map(Number);
    this.competencia.set(new Date(Date.UTC(ano, mes - 2, 1)).toISOString().substring(0, 10));
    this.carregarPeriodo();
  }

  mesSeguinte(): void {
    const [ano, mes] = this.competencia().split('-').map(Number);
    this.competencia.set(new Date(Date.UTC(ano, mes, 1)).toISOString().substring(0, 10));
    this.carregarPeriodo();
  }

  // ── Período: ações ────────────────────────────────────────────────────────
  abrirFormPeriodo(periodo?: DespesaPeriodoResponse): void {

    if (periodo) {
      this.editandoPeriodo.set(periodo);
      this.formPeriodo.setValue({
        descricao:      periodo.descricao,
        valorPlanejado: periodo.valorPlanejado,
        categoriaId:    periodo.categoriaId,
        despesaId:      periodo.despesaId ?? null,
      });
    } else {
      this.editandoPeriodo.set(null);
      const defaultCat = this.categorias()[0]?.id ?? '';
      this.formPeriodo.reset({ categoriaId: defaultCat, despesaId: null });
    }
    this.abaAtiva.set('periodo');
    this.showForm.set(true);
  }

  fecharForm(): void {
    this.showForm.set(false);
    this.editandoPeriodo.set(null);
    this.editandoTemplate.set(null);
  }

  salvarPeriodo(): void {
    if (this.formPeriodo.invalid || this.saving()) return;
    this.saving.set(true);


    const { descricao, valorPlanejado, categoriaId, despesaId } = this.formPeriodo.getRawValue();
    const id = this.editandoPeriodo()?.id;

    const req$ = id
      ? this.periodoService.atualizar(id, { descricao: descricao!, valorPlanejado: valorPlanejado!, categoriaId: categoriaId! })
      : this.periodoService.criar({ descricao: descricao!, valorPlanejado: valorPlanejado!, categoriaId: categoriaId!, despesaId: despesaId ?? undefined, competencia: this.competencia() });

    req$.subscribe({
      next: () => { this.fecharForm(); this.carregarPeriodo(); this.saving.set(false); },
      error: (err) => {
        const msg = err?.error?.errors?.[0] ?? 'Erro ao salvar.';
        this.notify.error(msg);
        this.saving.set(false);
      },
    });
  }

  pagar(periodo: DespesaPeriodoResponse): void {
    if (this.pagandoId()) return;
    this.pagandoId.set(periodo.id);
    this.periodoService.pagar(periodo.id, { dataPagamento: new Date().toISOString() }).subscribe({
      next: (updated) => { this.periodos.update(list => list.map(d => d.id === updated.id ? updated : d)); this.pagandoId.set(null); },
      error: () => this.pagandoId.set(null),
    });
  }

  deletarPeriodo(id: string): void {
    if (this.deletandoId()) return;
    this.deletandoId.set(id);
    this.periodoService.deletar(id).subscribe({
      next: () => { this.periodos.update(list => list.filter(d => d.id !== id)); this.deletandoId.set(null); },
      error: () => this.deletandoId.set(null),
    });
  }

  gerarPeriodo(): void {
    if (this.gerando()) return;
    this.gerando.set(true);
    this.periodoService.gerarPeriodo(this.competencia()).subscribe({
      next: (novos) => {
        if (novos.length === 0) {
          this.notify.warn('Nenhum template ativo encontrado ou período já gerado.');
        } else {
          this.notify.success(`${novos.length} lançamento(s) gerado(s) com sucesso.`);
        }
        this.carregarPeriodo();
        this.gerando.set(false);
      },
      error: (err) => {
        this.notify.error(err?.error?.errors?.[0] ?? 'Erro ao gerar período.');
        this.gerando.set(false);
      },
    });
  }

  anexarArquivoTemplate(template: DespesaResponse, file: File): void {
    this.enviarArquivo(template.id, file, template.id, (arquivo) => {
      this.templates.update((list) =>
        list.map((t) => t.id === template.id ? { ...t, arquivoId: arquivo.id } : t),
      );
      this.periodos.update((list) =>
        list.map((p) => p.despesaId === template.id ? { ...p, arquivoId: arquivo.id } : p),
      );
    });
  }

  anexarArquivoPeriodo(periodo: DespesaPeriodoResponse, file: File): void {
    const despesaId = periodo.despesaId;
    if (!despesaId) {
      this.notify.warn('Não é possível anexar arquivo a uma despesa avulsa sem template vinculado.');
      return;
    }
    this.enviarArquivo(despesaId, file, periodo.id, (arquivo) => {
      this.periodos.update((list) =>
        list.map((p) => p.id === periodo.id || p.despesaId === despesaId
          ? { ...p, arquivoId: arquivo.id }
          : p),
      );
    });
  }

  private enviarArquivo(
    despesaId: string,
    file: File,
    trackKey: string,
    onSuccess?: (arquivo: ArquivoResponse) => void,
  ): void {
    if (this.uploadingArquivoId()) return;
    this.uploadingArquivoId.set(trackKey);

    this.arquivoService.registrarArquivo(despesaId, file).subscribe({
      next: (arquivo) => {
        this.arquivosAnexados.update((map) => ({
          ...map,
          [trackKey]: arquivo,
          [despesaId]: arquivo,
        }));
        onSuccess?.(arquivo);
        this.notify.success(`Arquivo "${arquivo.nome}" anexado com sucesso.`);
        this.uploadingArquivoId.set(null);
      },
      error: (err) => {
        const msg = err?.error?.title ?? err?.error?.message ?? err?.error?.errors?.[0];
        this.notify.error(msg ?? 'Erro ao anexar arquivo.');
        this.uploadingArquivoId.set(null);
      },
    });
  }

  verArquivo(template: DespesaResponse): void {
    const arquivoId = this.resolverArquivoIdTemplate(template);
    if (arquivoId) this.abrirArquivo(arquivoId);
  }

  verArquivoPeriodo(periodo: DespesaPeriodoResponse): void {
    const arquivoId = this.resolverArquivoIdPeriodo(periodo);
    if (arquivoId) this.abrirArquivo(arquivoId);
  }

  private resolverArquivoIdTemplate(template: DespesaResponse): string | undefined {
    return template.arquivoId
      ?? this.arquivosAnexados()[template.id]?.id;
  }

  private resolverArquivoIdPeriodo(periodo: DespesaPeriodoResponse): string | undefined {
    return periodo.arquivoId
      ?? this.arquivosAnexados()[periodo.id]?.id
      ?? (periodo.despesaId ? this.arquivosAnexados()[periodo.despesaId]?.id : undefined)
      ?? (periodo.despesaId
        ? this.templates().find((t) => t.id === periodo.despesaId)?.arquivoId
        : undefined);
  }

  private abrirArquivo(arquivoId: string): void {
    this.fileViewer.open(arquivoId);
  }

  templateTemArquivo(template: DespesaResponse): boolean {
    return !!this.resolverArquivoIdTemplate(template);
  }

  periodoTemArquivo(periodo: DespesaPeriodoResponse): boolean {
    return !!this.resolverArquivoIdPeriodo(periodo);
  }

  // ── Templates: ações ──────────────────────────────────────────────────────
  abrirFormTemplate(template?: DespesaResponse): void {

    if (template) {
      this.editandoTemplate.set(template);
      this.formTemplate.setValue({
        nome:           template.nome,
        tipo:           (template.tipo === 'Fixa' ? 1 : template.tipo === 'Variavel' ? 2 : 3) as TipoDespesa,
        valorPlanejado: template.valorPlanejado,
        categoriaId:    template.categoriaId,
        diaVencimento:  template.diaVencimento ?? null,
      });
    } else {
      this.editandoTemplate.set(null);
      const defaultCat = this.categorias()[0]?.id ?? '';
      this.formTemplate.reset({ tipo: 3, categoriaId: defaultCat });
    }
    this.abaAtiva.set('templates');
    this.showForm.set(true);
  }

  salvarTemplate(): void {
    if (this.formTemplate.invalid || this.saving()) return;
    this.saving.set(true);


    const { nome, tipo, valorPlanejado, categoriaId, diaVencimento } = this.formTemplate.getRawValue();
    const payload = { nome: nome!, tipo: tipo!, valorPlanejado: valorPlanejado!, categoriaId: categoriaId!, diaVencimento: diaVencimento ?? undefined };
    const id = this.editandoTemplate()?.id;

    const req$ = id
      ? this.despesaService.atualizar(id, payload)
      : this.despesaService.criar(payload);

    req$.subscribe({
      next: () => { this.fecharForm(); this.carregarTemplates(1); this.saving.set(false); },
      error: (err) => {
        this.notify.error(err?.error?.errors?.[0] ?? 'Erro ao salvar conta fixa.');
        this.saving.set(false);
      },
    });
  }

  desativarTemplate(id: string): void {
    this.despesaService.desativar(id).subscribe({ next: () => this.carregarTemplates(this.tmPage()) });
  }

  deletarTemplate(id: string): void {
    if (this.deletandoId()) return;
    this.deletandoId.set(id);
    this.despesaService.deletar(id).subscribe({
      next: () => { this.carregarTemplates(this.tmPage()); this.deletandoId.set(null); },
      error: () => this.deletandoId.set(null),
    });
  }

  anexarComprovante(periodo: DespesaPeriodoResponse, file: File): void {
    const key = periodo.id + '-comprovante';
    if (this.uploadingArquivoId()) return;
    this.uploadingArquivoId.set(key);
    this.periodoService.uploadComprovante(periodo.id, file).subscribe({
      next: (updated) => {
        this.periodos.update(list => list.map(p => p.id === updated.id ? updated : p));
        this.notify.success('Comprovante anexado com sucesso.');
        this.uploadingArquivoId.set(null);
      },
      error: (err) => {
        this.notify.error(err?.error?.errors?.[0] ?? 'Erro ao anexar comprovante.');
        this.uploadingArquivoId.set(null);
      },
    });
  }

  gerarando = signal(false);

  gerarRelatorio(): void {
    if (this.gerarando()) return;
    this.gerarando.set(true);
    this.periodoService.gerarRelatorioMensal(this.competencia()).subscribe({
      next: (blob) => {
        const url  = URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href  = url;
        link.download = `relatorio-${this.competencia().substring(0, 7)}.pdf`;
        link.click();
        URL.revokeObjectURL(url);
        this.gerarando.set(false);
      },
      error: () => {
        this.notify.error('Erro ao gerar relatório. Tente novamente.');
        this.gerarando.set(false);
      },
    });
  }

  // ── Helpers ───────────────────────────────────────────────────────────────
  boletoUrl(path: string): string {
    return `${environment.apiUrl}${path}`;
  }

  formatarMoeda(valor: number | undefined): string {
    return (valor ?? 0).toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
  }

  formatarData(iso: string): string {
    return new Date(iso).toLocaleDateString('pt-BR');
  }

  vencida(d: DespesaPeriodoResponse): boolean {
    return !d.paga && new Date(d.competencia) < new Date(primeiroDiaMes(new Date()));
  }

  statusSeverity(d: DespesaPeriodoResponse): 'success' | 'warn' | 'danger' | 'secondary' {
    if (d.paga) return 'success';
    if (this.vencida(d)) return 'danger';
    return 'warn';
  }

  statusLabel(d: DespesaPeriodoResponse): string {
    if (d.paga) return 'Pago';
    if (this.vencida(d)) return 'Atrasado';
    return 'Pendente';
  }

  tipoSeverity(tipo: string): 'success' | 'info' | 'warn' | 'danger' | 'secondary' | 'contrast' {
    if (tipo === 'Fixa') return 'warn';
    if (tipo === 'Variavel') return 'info';
    return 'secondary';
  }
}

