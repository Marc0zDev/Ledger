import {
  Component, inject, OnInit, signal, computed
} from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { DespesaService } from '../../../core/services/despesa.service';
import { DespesaResponse } from '../../../core/models/cofre.model';
import { environment } from '../../../../environments/environment';

type Aba = 'todas' | 'pendentes';

export const CATEGORIAS_DESPESA = [
  { valor: 1,  label: 'Moradia'      },
  { valor: 2,  label: 'Alimentação'  },
  { valor: 3,  label: 'Transporte'   },
  { valor: 4,  label: 'Saúde'        },
  { valor: 5,  label: 'Educação'     },
  { valor: 6,  label: 'Lazer'        },
  { valor: 7,  label: 'Assinatura'   },
  { valor: 99, label: 'Outro'        },
];

@Component({
  selector: 'app-despesas',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './despesas.component.html',
  styleUrl: './despesas.component.scss',
})
export class DespesasComponent implements OnInit {
  private readonly despesaService = inject(DespesaService);
  private readonly fb             = inject(FormBuilder);

  readonly categorias = CATEGORIAS_DESPESA;

  // ── Estado ───────────────────────────────────────────────────────────────
  despesas    = signal<DespesaResponse[]>([]);
  loading     = signal(true);
  abaAtiva    = signal<Aba>('todas');
  editando    = signal<DespesaResponse | null>(null);
  showForm    = signal(false);
  saving      = signal(false);
  erros       = signal<string[]>([]);
  pagandoId   = signal<string | null>(null);
  deletandoId = signal<string | null>(null);

  // ── Computed ─────────────────────────────────────────────────────────────
  listagem = computed(() =>
    this.abaAtiva() === 'pendentes'
      ? this.despesas().filter(d => !d.paga)
      : this.despesas()
  );

  totalPendente  = computed(() => this.despesas().filter(d => !d.paga).reduce((s, d) => s + d.valor, 0));
  totalPago      = computed(() => this.despesas().filter(d =>  d.paga).reduce((s, d) => s + d.valor, 0));
  pendentesCount = computed(() => this.despesas().filter(d => !d.paga).length);

  // Agrupa listagem por mês/ano do vencimento
  grupos = computed(() => {
    const map = new Map<string, DespesaResponse[]>();
    for (const d of this.listagem()) {
      const dt  = new Date(d.dataVencimento);
      const key = dt.toLocaleString('pt-BR', { month: 'long', year: 'numeric' });
      if (!map.has(key)) map.set(key, []);
      map.get(key)!.push(d);
    }
    return Array.from(map.entries()).map(([mes, items]) => ({ mes, items }));
  });

  // ── Form ─────────────────────────────────────────────────────────────────
  form = this.fb.group({
    descricao:      ['', [Validators.required, Validators.maxLength(200)]],
    valor:          [null as number | null, [Validators.required, Validators.min(0.01)]],
    dataVencimento: [new Date().toISOString().substring(0, 10), Validators.required],
    categoria:      [99],
    recorrente:     [false],
  });

  // ── Lifecycle ─────────────────────────────────────────────────────────────
  ngOnInit(): void { this.carregar(); }

  // ── Actions ───────────────────────────────────────────────────────────────
  carregar(): void {
    this.loading.set(true);
    this.despesaService.listar().subscribe({
      next: (list) => { this.despesas.set(list); this.loading.set(false); },
      error: () => this.loading.set(false),
    });
  }

  abrirForm(despesa?: DespesaResponse): void {
    this.erros.set([]);
    if (despesa) {
      this.editando.set(despesa);
      this.form.setValue({
        descricao:      despesa.descricao,
        valor:          despesa.valor,
        dataVencimento: despesa.dataVencimento.substring(0, 10),
        categoria:      this.categoriaNumero(despesa.categoria),
        recorrente:     despesa.recorrente,
      });
    } else {
      this.editando.set(null);
      this.form.reset({ dataVencimento: new Date().toISOString().substring(0, 10), categoria: 99, recorrente: false });
    }
    this.showForm.set(true);
  }

  fecharForm(): void {
    this.showForm.set(false);
    this.editando.set(null);
    this.erros.set([]);
  }

  salvar(): void {
    if (this.form.invalid || this.saving()) return;
    this.saving.set(true);
    this.erros.set([]);

    const { descricao, valor, dataVencimento, categoria, recorrente } = this.form.getRawValue();
    const payload = {
      descricao: descricao!,
      valor: valor!,
      dataVencimento: new Date(dataVencimento!).toISOString(),
      categoria: categoria ?? 99,
      recorrente: recorrente ?? false,
    };

    const id = this.editando()?.id;
    const req$ = id
      ? this.despesaService.atualizar(id, payload)
      : this.despesaService.criar(payload);

    req$.subscribe({
      next: () => { this.fecharForm(); this.carregar(); this.saving.set(false); },
      error: (err) => {
        this.erros.set(err?.error?.errors ?? ['Erro ao salvar despesa.']);
        this.saving.set(false);
      },
    });
  }

  pagar(despesa: DespesaResponse): void {
    if (this.pagandoId()) return;
    this.pagandoId.set(despesa.id);
    this.despesaService.pagar(despesa.id, new Date().toISOString()).subscribe({
      next: (updated) => {
        this.despesas.update(list => list.map(d => d.id === updated.id ? updated : d));
        this.pagandoId.set(null);
      },
      error: () => this.pagandoId.set(null),
    });
  }

  deletar(id: string): void {
    if (this.deletandoId()) return;
    this.deletandoId.set(id);
    this.despesaService.deletar(id).subscribe({
      next: () => { this.despesas.update(list => list.filter(d => d.id !== id)); this.deletandoId.set(null); },
      error: () => this.deletandoId.set(null),
    });
  }

  onBoletoChange(event: Event, despesa: DespesaResponse): void {
    const file = (event.target as HTMLInputElement).files?.[0];
    if (!file) return;
    this.despesaService.uploadBoleto(despesa.id, file).subscribe({
      next: (updated) =>
        this.despesas.update(list => list.map(d => d.id === updated.id ? updated : d)),
    });
  }

  boletoUrl(path: string): string {
    return `${environment.apiUrl}${path}`;
  }

  categoriaNumero(nome: string): number {
    return this.categorias.find(c => c.label.toLowerCase() === nome?.toLowerCase())?.valor ?? 99;
  }

  formatarMoeda(valor: number | undefined): string {
    return (valor ?? 0).toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
  }

  formatarData(iso: string): string {
    return new Date(iso).toLocaleDateString('pt-BR');
  }

  vencida(d: DespesaResponse): boolean {
    return !d.paga && new Date(d.dataVencimento) < new Date();
  }
}


