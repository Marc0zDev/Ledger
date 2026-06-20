import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { GrupoService } from '../../../core/services/grupo.service';
import { UsuarioService } from '../../../core/services/usuario.service';
import { AuthService } from '../../../core/services/auth.service';
import { NotificationService } from '../../../core/services/notification.service';
import { GrupoResponse, GrupoMembroResponse } from '../../../core/models/grupo.model';
import { DespesaPeriodoResponse, UsuarioResponse } from '../../../core/models/cofre.model';
import { ReceitaResponse } from '../../../core/models/receita.model';
import { LedgerTableComponent, LedgerColumn } from '../../../shared/components/ledger-table/ledger-table.component';

@Component({
  selector: 'app-grupo-detalhe',
  standalone: true,
  imports: [RouterLink, ReactiveFormsModule, LedgerTableComponent],
  templateUrl: './grupo-detalhe.component.html',
  styleUrl: './grupo-detalhe.component.scss',
})
export class GrupoDetalheComponent implements OnInit {
  private readonly route         = inject(ActivatedRoute);
  private readonly grupoService  = inject(GrupoService);
  private readonly usuarioService = inject(UsuarioService);
  private readonly auth          = inject(AuthService);
  private readonly notify        = inject(NotificationService);
  private readonly fb            = inject(FormBuilder);

  grupo   = signal<GrupoResponse | null>(null);
  loading = signal(true);
  erro    = signal<string | null>(null);

  // Edição do grupo
  showEditForm = signal(false);
  saving       = signal(false);

  editForm = this.fb.nonNullable.group({
    nome:      ['', [Validators.required, Validators.maxLength(100)]],
    descricao: [''],
  });

  // Adição de membro
  showAddMembro    = signal(false);
  buscandoUsuario  = signal(false);
  savingMembro     = signal(false);
  usuarioEncontrado = signal<UsuarioResponse | null>(null);

  emailForm = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
  });

  // ── Finanças ──────────────────────────────────────────────────────────────
  competencia    = signal<Date>(new Date(Date.UTC(new Date().getFullYear(), new Date().getMonth(), 1)));
  despesas       = signal<DespesaPeriodoResponse[]>([]);
  receitas       = signal<ReceitaResponse[]>([]);
  loadingFinancas = signal(false);
  abaFinancas    = signal<'despesas' | 'receitas'>('despesas');

  competenciaLabel = computed(() =>
    this.competencia().toLocaleString('pt-BR', { month: 'long', year: 'numeric', timeZone: 'UTC' })
  );

  totalDespesasPlanejadas = computed(() =>
    this.despesas().reduce((s, d) => s + d.valorPlanejado, 0));

  totalDespesasRealizadas = computed(() =>
    this.despesas().filter(d => d.paga).reduce((s, d) => s + d.valorRealizado, 0));

  totalReceitas = computed(() =>
    this.receitas().reduce((s, r) => s + r.valor, 0));

  saldo = computed(() => this.totalReceitas() - this.totalDespesasPlanejadas());

  despesaColumns: LedgerColumn[] = [
    { title: 'Membro',    field: 'usuarioNome',    width: '18%' },
    { title: 'Descrição', field: 'descricao',      width: '22%' },
    { title: 'Categoria', field: 'categoriaNome',  width: '18%' },
    { title: 'Valor',     field: 'valorPlanejado', type: 'currency', width: '18%', align: 'right' },
    {
      title: 'Status', type: 'tag', width: '14%',
      tagLabel:    (_, row) => (row as DespesaPeriodoResponse).paga ? 'Pago' : 'Pendente',
      tagSeverity: (_, row) => (row as DespesaPeriodoResponse).paga ? 'success' : 'warn',
    },
  ];

  receitaColumns: LedgerColumn[] = [
    { title: 'Membro',    field: 'usuarioNome',    width: '20%' },
    { title: 'Nome',      field: 'nome',           width: '25%' },
    { title: 'Valor',     field: 'valor', type: 'currency', width: '20%', align: 'right' },
    { title: 'Recebido',  field: 'dataRecebimento', type: 'date', width: '20%' },
  ];

  private grupoId!: string;

  get usuarioId() { return this.auth.currentUser()!.usuarioId; }

  isChefe = computed(() =>
    this.grupo()?.membros.some(m => m.usuarioId === this.usuarioId && m.role === 'Chefe') ?? false
  );

  ngOnInit(): void {
    this.grupoId = this.route.snapshot.paramMap.get('id')!;
    this.carregar();
    this.carregarFinancas();
  }

  carregar(): void {
    this.grupoService.obterPorId(this.grupoId).subscribe({
      next:  (data) => { this.grupo.set(data); this.loading.set(false); },
      error: ()     => { this.erro.set('Grupo não encontrado.'); this.loading.set(false); },
    });
  }

  // ── Editar grupo ─────────────────────────────────────────────────────────

  abrirEditForm(): void {
    const g = this.grupo()!;
    this.editForm.setValue({ nome: g.nome, descricao: g.descricao ?? '' });
    this.showEditForm.set(true);
  }

  fecharEditForm(): void {
    this.showEditForm.set(false);
    this.editForm.reset();
  }

  salvarEdicao(): void {
    if (this.editForm.invalid || this.saving()) return;
    this.saving.set(true);
    const { nome, descricao } = this.editForm.getRawValue();
    this.grupoService.atualizar(this.grupoId, { nome, descricao: descricao || undefined }).subscribe({
      next: (g) => {
        this.grupo.set(g);
        this.notify.success('Grupo atualizado.');
        this.fecharEditForm();
        this.saving.set(false);
      },
      error: (err) => {
        this.notify.error(err?.error?.errors?.[0] ?? 'Erro ao atualizar grupo.');
        this.saving.set(false);
      },
    });
  }

  // ── Adicionar membro ─────────────────────────────────────────────────────

  buscarUsuario(): void {
    const email = this.emailForm.get('email')?.value?.trim();
    if (!email) return;
    this.buscandoUsuario.set(true);
    this.usuarioEncontrado.set(null);
    this.usuarioService.obterPorEmail(email).subscribe({
      next: (u) => { this.usuarioEncontrado.set(u); this.buscandoUsuario.set(false); },
      error: ()  => {
        this.notify.error('Usuário não encontrado com este e-mail.');
        this.buscandoUsuario.set(false);
      },
    });
  }

  adicionarMembro(): void {
    const usuario = this.usuarioEncontrado();
    if (!usuario || this.savingMembro()) return;
    this.savingMembro.set(true);
    this.grupoService.adicionarMembro(this.grupoId, { usuarioId: usuario.id }).subscribe({
      next: () => {
        this.notify.success(`Convite enviado para ${usuario.email}.`);
        this.fecharAddMembro();
        this.savingMembro.set(false);
      },
      error: (err) => {
        this.notify.error(err?.error?.errors?.[0] ?? 'Erro ao enviar convite.');
        this.savingMembro.set(false);
      },
    });
  }

  fecharAddMembro(): void {
    this.showAddMembro.set(false);
    this.emailForm.reset();
    this.usuarioEncontrado.set(null);
  }

  // ── Remover membro ───────────────────────────────────────────────────────

  removerMembro(membro: GrupoMembroResponse): void {
    this.grupoService.removerMembro(this.grupoId, membro.id).subscribe({
      next: () => {
        this.notify.success(`${membro.nome} removido do grupo.`);
        this.carregar();
      },
      error: (err) => this.notify.error(err?.error?.errors?.[0] ?? 'Erro ao remover membro.'),
    });
  }

  // ── Finanças do grupo ──────────────────────────────────────────────────────

  carregarFinancas(): void {
    this.loadingFinancas.set(true);
    const comp = this.competencia().toISOString().substring(0, 10);

    this.grupoService.listarDespesas(this.grupoId, comp).subscribe({
      next: (d) => this.despesas.set(d),
      error: () => this.despesas.set([]),
    });

    this.grupoService.listarReceitas(this.grupoId, comp).subscribe({
      next: (r) => { this.receitas.set(r); this.loadingFinancas.set(false); },
      error: () => { this.receitas.set([]); this.loadingFinancas.set(false); },
    });
  }

  mesAnterior(): void {
    const d = this.competencia();
    this.competencia.set(new Date(Date.UTC(d.getUTCFullYear(), d.getUTCMonth() - 1, 1)));
    this.carregarFinancas();
  }

  mesSeguinte(): void {
    const d = this.competencia();
    this.competencia.set(new Date(Date.UTC(d.getUTCFullYear(), d.getUTCMonth() + 1, 1)));
    this.carregarFinancas();
  }

  formatarMoeda(valor: number): string {
    return valor.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
  }

  formatarData(data: string): string {
    return new Date(data).toLocaleDateString('pt-BR');
  }
}
