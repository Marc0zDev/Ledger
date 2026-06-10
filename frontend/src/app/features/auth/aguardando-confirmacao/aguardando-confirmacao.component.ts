import { Component, inject, signal, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-aguardando-confirmacao',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './aguardando-confirmacao.component.html',
  styleUrl: './aguardando-confirmacao.component.scss',
})
export class AguardandoConfirmacaoComponent implements OnInit, OnDestroy {
  private readonly route  = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly http   = inject(HttpClient);

  email    = signal('');
  userId   = signal('');
  tentando = signal(false);

  private intervalId?: ReturnType<typeof setInterval>;

  ngOnInit(): void {
    const params = this.route.snapshot.queryParamMap;
    this.email.set(params.get('email') ?? '');
    this.userId.set(params.get('userId') ?? '');

    if (!this.userId()) {
      this.router.navigate(['/login']);
      return;
    }

    this.iniciarPolling();
  }

  ngOnDestroy(): void {
    clearInterval(this.intervalId);
  }

  private iniciarPolling(): void {
    this.intervalId = setInterval(() => this.verificar(), 4000);
  }

  private verificar(): void {
    if (this.tentando()) return;
    this.tentando.set(true);

    this.http
      .get<{ confirmado: boolean }>(
        `${environment.apiUrl}/api/auth/email-confirmado?userId=${this.userId()}`
      )
      .subscribe({
        next: (res) => {
          this.tentando.set(false);
          if (res.confirmado) {
            clearInterval(this.intervalId);
            this.router.navigate(['/login'], { queryParams: { confirmed: 'true' } });
          }
        },
        error: () => this.tentando.set(false),
      });
  }
}
