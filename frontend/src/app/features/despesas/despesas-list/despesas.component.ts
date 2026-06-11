import {
  Component, inject, OnInit, signal, computed
} from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { DespesaService } from '../../../core/services/despesa.service';
import { DespesaPeriodoService } from '../../../core/services/despesa-periodo.service';
import { CategoriaService } from '../../../core/services/categoria.service';
import {
  DespesaResponse,
  DespesaPeriodoResponse,
  CategoriaResponse,
  TipoDespesa,
} from '../../../core/models/cofre.model';
import { environment } from '../../../../environments/environment';

type Aba = 'periodo' | 'templates';

function primeiroDiaMes(d: Date): string {
  return new Date(Date.UTC(d.getFullYear(), d.getMonth(), 1)).toISOString().substring(0, 10);
}

@Component({
  selector: 'app-despesas',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './despesas.component.html',
  styleUrl: './despesas.component.scss',
})
export class DespesasComponent implements OnInit {
  private readonly despesaService       = inject(DespesaService);
  private readonly periodoService       = inject(DespesaPeriodoService);
  private readonly categoriaService     = inject(CategoriaService);
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
  erros         = signal<string[]>([]);
  editandoPeriodo  = signal<DespesaPeriodoResponse | null>(null);
  editandoTemplate = signal<DespesaResponse | null>(null);
  pagandoId     = signal<string | null>(null);
  deletandoId   = signal<string | null>(null);
  gerando       = signal(false);

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

  carregarTemplates(): void {
    this.despesaService.listar().subscribe({
      next: (list) => this.templates.set(list),
    });
  }

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
    this.erros.set([]);
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
    this.erros.set([]);
  }

  salvarPeriodo(): void {
    if (this.formPeriodo.invalid || this.saving()) return;
    this.saving.set(true);
    this.erros.set([]);

    const { descricao, valorPlanejado, categoriaId, despesaId } = this.formPeriodo.getRawValue();
    const id = this.editandoPeriodo()?.id;

    const req$ = id
      ? this.periodoService.atualizar(id, { descricao: descricao!, valorPlanejado: valorPlanejado!, categoriaId: categoriaId! })
      : this.periodoService.criar({ descricao: descricao!, valorPlanejado: valorPlanejado!, categoriaId: categoriaId!, despesaId: despesaId ?? undefined, competencia: this.competencia() });

    req$.subscribe({
      next: () => { this.fecharForm(); this.carregarPeriodo(); this.saving.set(false); },
      error: (err) => { this.erros.set(err?.error?.errors ?? ['Erro ao salvar.']); this.saving.set(false); },
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

  onBoletoChange(event: Event, periodo: DespesaPeriodoResponse): void {
    const file = (event.target as HTMLInputElement).files?.[0];
    if (!file) return;
    this.periodoService.uploadBoleto(periodo.id, file).subscribe({
      next: (updated) => this.periodos.update(list => list.map(d => d.id === updated.id ? updated : d)),
    });
  }

  gerarPeriodo(): void {
    if (this.gerando()) return;
    this.gerando.set(true);
    this.periodoService.gerarPeriodo(this.competencia()).subscribe({
      next: (novos) => {
        if (novos.length === 0) {
          this.erros.set(['Nenhum template ativo encontrado ou período já gerado.']);
        }
        this.carregarPeriodo();
        this.gerando.set(false);
      },
      error: (err) => { this.erros.set(err?.error?.errors ?? ['Erro ao gerar período.']); this.gerando.set(false); },
    });
  }

  // ── Templates: ações ──────────────────────────────────────────────────────
  abrirFormTemplate(template?: DespesaResponse): void {
    this.erros.set([]);
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
    this.erros.set([]);

    const { nome, tipo, valorPlanejado, categoriaId, diaVencimento } = this.formTemplate.getRawValue();
    const payload = { nome: nome!, tipo: tipo!, valorPlanejado: valorPlanejado!, categoriaId: categoriaId!, diaVencimento: diaVencimento ?? undefined };
    const id = this.editandoTemplate()?.id;

    const req$ = id
      ? this.despesaService.atualizar(id, payload)
      : this.despesaService.criar(payload);

    req$.subscribe({
      next: () => { this.fecharForm(); this.carregarTemplates(); this.saving.set(false); },
      error: (err) => { this.erros.set(err?.error?.errors ?? ['Erro ao salvar template.']); this.saving.set(false); },
    });
  }

  desativarTemplate(id: string): void {
    this.despesaService.desativar(id).subscribe({ next: () => this.carregarTemplates() });
  }

  deletarTemplate(id: string): void {
    if (this.deletandoId()) return;
    this.deletandoId.set(id);
    this.despesaService.deletar(id).subscribe({
      next: () => { this.templates.update(list => list.filter(t => t.id !== id)); this.deletandoId.set(null); },
      error: () => this.deletandoId.set(null),
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
}

