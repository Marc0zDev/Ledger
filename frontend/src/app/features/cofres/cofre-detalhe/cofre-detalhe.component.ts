import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { CofreService } from '../../../core/services/cofre.service';
import { UsuarioService } from '../../../core/services/usuario.service';
import { AuthService } from '../../../core/services/auth.service';
import { NotificationService } from '../../../core/services/notification.service';
import {
  CofreResponse, MovimentacaoResponse, PagedResult, ParticipanteResponse, UsuarioResponse
} from '../../../core/models/cofre.model';

@Component({
  selector: 'app-cofre-detalhe',
  standalone: true,
  imports: [RouterLink, ReactiveFormsModule],
  templateUrl: './cofre-detalhe.component.html',
  styleUrl: './cofre-detalhe.component.scss',
})
export class CofreDetalheComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly cofreService = inject(CofreService);
  private readonly usuarioService = inject(UsuarioService);
  private readonly auth = inject(AuthService);
  private readonly notify = inject(NotificationService);
  private readonly fb = inject(FormBuilder);

  cofre = signal<CofreResponse | null>(null);
  participantes = signal<ParticipanteResponse[]>([]);
  movimentacoes = signal<MovimentacaoResponse[]>([]);

  isAdmin = computed(() => {
    const userId = this.auth.currentUser()?.usuarioId;
    const parts = this.participantes();
    const mine = parts.find(p => p.usuarioId === userId);
    // Criador do cofre sem registro explícito ou com role Admin
    return !mine || mine.role === 'Admin';
  });
  // Paginação server-side
  readonly pageSize = 5;
  movPage       = signal(1);
  movTotal      = signal(0);
  movTotalPags  = signal(1);
  movCarregando = signal(false);
  movPodeAnterior = computed(() => this.movPage() > 1);
  movPodeProxima  = computed(() => this.movPage() < this.movTotalPags());
  loading = signal(true);
  erro = signal<string | null>(null);

  // Painéis
  showFormParticipante = signal(false);
  showFormMovimentacao = signal(false);

  // Estado dos forms
  savingParticipante = signal(false);
  errosParticipante = signal<string[]>([]);
  savingMovimentacao = signal(false);
  errosMovimentacao = signal<string[]>([]);

  // Usuário encontrado pelo email (lookup)
  usuarioEncontrado = signal<UsuarioResponse | null>(null);
  buscandoUsuario = signal(false);

  formParticipante = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
  });

  formMovimentacao = this.fb.group({
    descricao: ['', Validators.required],
    valor: [0, [Validators.required, Validators.min(0.01)]],
    tipo: ['Entrada', Validators.required],
    data: [new Date().toISOString().substring(0, 10), Validators.required],
  });

  private cofreId!: string;

  ngOnInit(): void {
    this.cofreId = this.route.snapshot.paramMap.get('id')!;
    this.carregar();
  }

  private carregar(): void {
    this.cofreService.obterComDetalhes(this.cofreId).subscribe({
      next: (data) => {
        this.cofre.set(data);
        this.participantes.set(data.participantes ?? []);
        this.loading.set(false);
        this.recarregarMovimentacoes(1);
      },
      error: () => {
        this.erro.set('Não foi possível carregar o cofre.');
        this.loading.set(false);
      },
    });
  }

  private recarregarCofre(): void {
    this.cofreService.obterComDetalhes(this.cofreId).subscribe({
      next: (data) => this.cofre.set(data),
    });
  }

  private recarregarParticipantes(): void {
    this.cofreService.listarParticipantes(this.cofre()!.id).subscribe({
      next: (list) => this.participantes.set(list),
    });
  }

  private recarregarMovimentacoes(page = this.movPage()): void {
    this.movCarregando.set(true);
    this.cofreService.listarMovimentacoes(this.cofre()!.id, page, this.pageSize).subscribe({
      next: (r) => {
        this.movimentacoes.set(r.items ?? []);
        this.movPage.set(r.page ?? 1);
        this.movTotal.set(r.total ?? 0);
        this.movTotalPags.set(r.totalPages ?? 1);
        this.movCarregando.set(false);
      },
      error: () => { this.movCarregando.set(false); },
    });
  }

  buscarUsuario(): void {
    const email = this.formParticipante.get('email')?.value?.trim();
    if (!email) return;
    this.buscandoUsuario.set(true);
    this.usuarioEncontrado.set(null);
    this.usuarioService.obterPorEmail(email).subscribe({
      next: (u) => { this.usuarioEncontrado.set(u); this.buscandoUsuario.set(false); },
      error: () => {
        this.errosParticipante.set(['Usuário não encontrado com este e-mail.']);
        this.buscandoUsuario.set(false);
      },
    });
  }

  adicionarParticipante(): void {
    const usuario = this.usuarioEncontrado();
    if (!usuario || this.savingParticipante()) return;
    this.errosParticipante.set([]);
    this.savingParticipante.set(true);
    this.cofreService.adicionarParticipante(this.cofre()!.id, { usuarioId: usuario.id }).subscribe({
      next: () => {
        this.recarregarParticipantes();
        this.formParticipante.reset();
        this.usuarioEncontrado.set(null);
        this.showFormParticipante.set(false);
        this.savingParticipante.set(false);
      },
      error: (err) => {
        this.errosParticipante.set(err?.error?.errors ?? ['Erro ao adicionar participante.']);
        this.savingParticipante.set(false);
      },
    });
  }

  registrarMovimentacao(): void {
    if (this.formMovimentacao.invalid || this.savingMovimentacao()) return;
    this.errosMovimentacao.set([]);
    this.savingMovimentacao.set(true);
    const v = this.formMovimentacao.value;
    this.cofreService.registrarMovimentacao(this.cofre()!.id, {
      descricao: v.descricao!,
      valor: v.valor!,
      tipo: v.tipo!,
      data: v.data!,
    }).subscribe({
      next: (mov) => {
        this.recarregarCofre();        // re-busca totalMovimentado correto do servidor
        this.recarregarMovimentacoes(1);
        this.formMovimentacao.reset({
          descricao: '', valor: 0, tipo: 'Entrada',
          data: new Date().toISOString().substring(0, 10),
        });
        this.showFormMovimentacao.set(false);
        this.savingMovimentacao.set(false);
      },
      error: (err) => {
        this.errosMovimentacao.set(err?.error?.errors ?? ['Erro ao registrar movimentação.']);
        this.savingMovimentacao.set(false);
      },
    });
  }

  movAnterior(): void { if (this.movPodeAnterior()) this.recarregarMovimentacoes(this.movPage() - 1); }
  movProxima():  void { if (this.movPodeProxima())  this.recarregarMovimentacoes(this.movPage() + 1); }

  aprovarMovimentacao(mov: MovimentacaoResponse): void {
    this.cofreService.aprovarMovimentacao(this.cofre()!.id, mov.id).subscribe({
      next: () => {
        this.notify.success('Movimentação aprovada.');
        this.recarregarCofre();
        this.recarregarMovimentacoes();
      },
      error: (err) => this.notify.error(err?.error?.errors?.[0] ?? 'Erro ao aprovar movimentação.'),
    });
  }

  rejeitarMovimentacao(mov: MovimentacaoResponse): void {
    this.cofreService.rejeitarMovimentacao(this.cofre()!.id, mov.id).subscribe({
      next: () => {
        this.notify.warn('Movimentação rejeitada.');
        this.recarregarMovimentacoes();
      },
      error: (err) => this.notify.error(err?.error?.errors?.[0] ?? 'Erro ao rejeitar movimentação.'),
    });
  }

  progresso(cofre: CofreResponse): number {
    if (!cofre.meta || cofre.meta === 0) return 0;
    return Math.min(Math.round((cofre.totalMovimentado / cofre.meta) * 100), 100);
  }

  formatarMoeda(valor: number | undefined | null): string {
    if (valor === undefined || valor === null) return 'R$\u00a00,00';
    return valor.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
  }

  formatarData(data: string): string {
    return new Date(data).toLocaleDateString('pt-BR');
  }
}
