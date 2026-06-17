import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { GrupoService } from '../../../core/services/grupo.service';
import { UsuarioService } from '../../../core/services/usuario.service';
import { AuthService } from '../../../core/services/auth.service';
import { NotificationService } from '../../../core/services/notification.service';
import { GrupoResponse, GrupoMembroResponse } from '../../../core/models/grupo.model';
import { UsuarioResponse } from '../../../core/models/cofre.model';

@Component({
  selector: 'app-grupo-detalhe',
  standalone: true,
  imports: [RouterLink, ReactiveFormsModule],
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

  private grupoId!: string;

  get usuarioId() { return this.auth.currentUser()!.usuarioId; }

  isChefe = computed(() =>
    this.grupo()?.membros.some(m => m.usuarioId === this.usuarioId && m.role === 'Chefe') ?? false
  );

  ngOnInit(): void {
    this.grupoId = this.route.snapshot.paramMap.get('id')!;
    this.carregar();
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
        this.notify.success(`${usuario.nome} adicionado ao grupo.`);
        this.fecharAddMembro();
        this.carregar();
        this.savingMembro.set(false);
      },
      error: (err) => {
        this.notify.error(err?.error?.errors?.[0] ?? 'Erro ao adicionar membro.');
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

  formatarData(data: string): string {
    return new Date(data).toLocaleDateString('pt-BR');
  }
}
