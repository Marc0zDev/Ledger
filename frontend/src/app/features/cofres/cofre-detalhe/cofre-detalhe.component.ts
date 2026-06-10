import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { CofreService } from '../../../core/services/cofre.service';
import { UsuarioService } from '../../../core/services/usuario.service';
import { CofreResponse, ParticipanteResponse, DespesaResponse, UsuarioResponse } from '../../../core/models/cofre.model';

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
  private readonly fb = inject(FormBuilder);

  cofre = signal<CofreResponse | null>(null);
  participantes = signal<ParticipanteResponse[]>([]);
  despesas = signal<DespesaResponse[]>([]);
  loading = signal(true);
  erro = signal<string | null>(null);

  // Painéis
  showFormParticipante = signal(false);
  showFormDespesa = signal(false);

  // Estado dos forms
  savingParticipante = signal(false);
  savingDespesa = signal(false);
  errosParticipante = signal<string[]>([]);
  errosDespesa = signal<string[]>([]);

  // Usuário encontrado pelo email (lookup)
  usuarioEncontrado = signal<UsuarioResponse | null>(null);
  buscandoUsuario = signal(false);

  formParticipante = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
  });

  formDespesa = this.fb.group({
    descricao:      ['', [Validators.required, Validators.maxLength(200)]],
    valor:          [null as number | null, [Validators.required, Validators.min(0.01)]],
    dataVencimento: [new Date().toISOString().substring(0, 10), Validators.required],
  });

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.carregar(id);
  }

  private carregar(id: string): void {
    this.cofreService.obterComDetalhes(id).subscribe({
      next: (data) => {
        this.cofre.set(data);
        this.participantes.set(data.participantes ?? []);
        this.loading.set(false);
      },
      error: () => {
        this.erro.set('Não foi possível carregar o cofre.');
        this.loading.set(false);
      },
    });
    this.cofreService.listarDespesas(id).subscribe({
      next: (list) => this.despesas.set(list),
    });
  }

  private recarregarParticipantes(): void {
    this.cofreService.listarParticipantes(this.cofre()!.id).subscribe({
      next: (list) => this.participantes.set(list),
    });
  }

  private recarregarDespesas(): void {
    const id = this.cofre()!.id;
    this.cofreService.listarDespesas(id).subscribe({
      next: (list) => this.despesas.set(list),
    });
    this.cofreService.obterComDetalhes(id).subscribe({
      next: (c) => this.cofre.set(c),
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

  registrarDespesa(): void {
    if (this.formDespesa.invalid || this.savingDespesa()) return;
    this.errosDespesa.set([]);
    this.savingDespesa.set(true);
    const { descricao, valor, dataVencimento } = this.formDespesa.getRawValue();
    this.cofreService.registrarDespesa(this.cofre()!.id, {
      descricao: descricao!,
      valor: valor!,
      dataVencimento: new Date(dataVencimento!).toISOString(),
    }).subscribe({
      next: () => {
        this.recarregarDespesas();
        this.formDespesa.reset({ dataVencimento: new Date().toISOString().substring(0, 10) });
        this.showFormDespesa.set(false);
        this.savingDespesa.set(false);
      },
      error: (err) => {
        this.errosDespesa.set(err?.error?.errors ?? ['Erro ao registrar despesa.']);
        this.savingDespesa.set(false);
      },
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
