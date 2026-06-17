import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { GrupoService } from '../../../core/services/grupo.service';
import { AuthService } from '../../../core/services/auth.service';
import { NotificationService } from '../../../core/services/notification.service';
import { GrupoResponse } from '../../../core/models/grupo.model';

@Component({
  selector: 'app-grupos-list',
  standalone: true,
  imports: [RouterLink, ReactiveFormsModule],
  templateUrl: './grupos-list.component.html',
  styleUrl: './grupos-list.component.scss',
})
export class GruposListComponent implements OnInit {
  private readonly grupoService = inject(GrupoService);
  private readonly auth         = inject(AuthService);
  private readonly notify       = inject(NotificationService);
  private readonly fb           = inject(FormBuilder);

  grupos   = signal<GrupoResponse[]>([]);
  loading  = signal(true);
  showForm = signal(false);
  saving   = signal(false);

  form = this.fb.nonNullable.group({
    nome:      ['', [Validators.required, Validators.maxLength(100)]],
    descricao: [''],
  });

  get usuarioId() { return this.auth.currentUser()!.usuarioId; }

  ngOnInit(): void { this.carregar(); }

  carregar(): void {
    this.loading.set(true);
    this.grupoService.listar().subscribe({
      next:  (data) => { this.grupos.set(data); this.loading.set(false); },
      error: ()     => { this.notify.error('Erro ao carregar grupos.'); this.loading.set(false); },
    });
  }

  abrirForm(): void {
    this.form.reset();
    this.showForm.set(true);
  }

  fecharForm(): void {
    this.showForm.set(false);
    this.form.reset();
  }

  criar(): void {
    if (this.form.invalid || this.saving()) return;
    this.saving.set(true);
    const { nome, descricao } = this.form.getRawValue();
    this.grupoService.criar({ nome, descricao: descricao || undefined }).subscribe({
      next: () => {
        this.notify.success('Grupo criado com sucesso.');
        this.fecharForm();
        this.carregar();
        this.saving.set(false);
      },
      error: (err) => {
        this.notify.error(err?.error?.errors?.[0] ?? 'Erro ao criar grupo.');
        this.saving.set(false);
      },
    });
  }

  isChefe(grupo: GrupoResponse): boolean {
    return grupo.membros.some(m => m.usuarioId === this.usuarioId && m.role === 'Chefe');
  }

  formatarData(data: string): string {
    return new Date(data).toLocaleDateString('pt-BR');
  }
}
