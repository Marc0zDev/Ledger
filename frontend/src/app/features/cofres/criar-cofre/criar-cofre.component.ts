import { Component, inject, signal } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { CofreService } from '../../../core/services/cofre.service';

import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { TextareaModule } from 'primeng/textarea';
import { MessageModule } from 'primeng/message';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { CardModule } from 'primeng/card';

@Component({
  selector: 'app-criar-cofre',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    ButtonModule,
    InputTextModule,
    InputNumberModule,
    TextareaModule,
    MessageModule,
    ProgressSpinnerModule,
    CardModule,
  ],
  templateUrl: './criar-cofre.component.html',
  styleUrl: './criar-cofre.component.scss',
})
export class CriarCofreComponent {
  private readonly fb = inject(FormBuilder);
  private readonly cofreService = inject(CofreService);
  private readonly router = inject(Router);

  loading = signal(false);
  success = signal(false);
  serverErrors = signal<string[]>([]);

  form = this.fb.group({
    nome: ['', [Validators.required, Validators.maxLength(150)]],
    meta: [null as number | null, [Validators.required, Validators.min(0.01)]],
    descricao: [''],
  });

  get nome() { return this.form.controls.nome; }
  get meta() { return this.form.controls.meta; }

  onSubmit(): void {
    if (this.form.invalid || this.loading()) return;

    this.serverErrors.set([]);
    this.loading.set(true);

    const { nome, meta, descricao } = this.form.getRawValue();

    this.cofreService.criar({ nome: nome!, meta: meta!, descricao: descricao ?? undefined }).subscribe({
      next: (cofre) => {
        this.loading.set(false);
        this.success.set(true);
        setTimeout(() => this.router.navigate(['/cofres', cofre.id]), 1200);
      },
      error: (err) => {
        this.loading.set(false);
        const errors: string[] = err?.error?.errors ?? ['Ocorreu um erro inesperado.'];
        this.serverErrors.set(errors);
      },
    });
  }
}
