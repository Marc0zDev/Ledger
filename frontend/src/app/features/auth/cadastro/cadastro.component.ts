import { Component, inject, signal } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';import { HttpErrorResponse } from '@angular/common/http';import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-cadastro',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './cadastro.component.html',
  styleUrl: './cadastro.component.scss',
})
export class CadastroComponent {
  private readonly authService = inject(AuthService);
  private readonly fb = inject(FormBuilder);

  loading = signal(false);
  erros = signal<string[]>([]);

  form = this.fb.group({
    nome:  ['', [Validators.required, Validators.maxLength(100)]],
    email: ['', [Validators.required, Validators.email]],
    senha: ['', [Validators.required, Validators.minLength(6)]],
  });

  cadastrar(): void {
    if (this.form.invalid || this.loading()) return;
    this.erros.set([]);
    this.loading.set(true);
    const { nome, email, senha } = this.form.getRawValue();
    this.authService.registrar({ nome: nome!, email: email!, senha: senha! }).subscribe({
      next: () => this.loading.set(false),
      error: (err: HttpErrorResponse) => {
        this.erros.set(err?.error?.errors ?? ['Erro ao criar conta.']);
        this.loading.set(false);
      },
    });
  }
}
