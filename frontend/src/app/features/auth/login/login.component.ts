import { Component, inject, signal, OnInit } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { RouterLink, ActivatedRoute } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent implements OnInit {
  private readonly authService = inject(AuthService);
  private readonly route       = inject(ActivatedRoute);
  private readonly fb          = inject(FormBuilder);

  loading          = signal(false);
  erros            = signal<string[]>([]);
  emailConfirmado  = signal(false);

  form = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    senha: ['', [Validators.required, Validators.minLength(6)]],
  });

  ngOnInit(): void {
    const confirmed = this.route.snapshot.queryParamMap.get('confirmed');
    if (confirmed === 'true') this.emailConfirmado.set(true);
  }

  entrar(): void {
    if (this.form.invalid || this.loading()) return;
    this.erros.set([]);
    this.loading.set(true);
    const { email, senha } = this.form.getRawValue();
    this.authService.login({ email: email!, senha: senha! }).subscribe({
      next: () => this.loading.set(false),
      error: (err: HttpErrorResponse) => {
        this.erros.set(err?.error?.errors ?? ['Erro ao fazer login.']);
        this.loading.set(false);
      },
    });
  }
}
