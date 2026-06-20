import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ConviteService } from '../../../core/services/convite.service';

@Component({
  selector: 'app-convite-aceitar',
  standalone: true,
  template: `
    <div class="convite-page">
      @if (loading()) {
        <p class="msg">Processando convite...</p>
      } @else if (erro()) {
        <p class="msg erro">{{ erro() }}</p>
        <button class="btn" (click)="router.navigate(['/'])">Ir para o início</button>
      } @else {
        <p class="msg sucesso">{{ mensagemSucesso() }}</p>
        <button class="btn" (click)="router.navigate([redirectPath()])">{{ botaoTexto() }}</button>
      }
    </div>
  `,
  styles: [`
    .convite-page { display:flex; flex-direction:column; align-items:center; justify-content:center; min-height:60vh; gap:1.5rem; }
    .msg { font-size:1.1rem; }
    .erro { color: #ef4444; }
    .sucesso { color: #22c55e; }
    .btn { padding:.6rem 1.4rem; background:var(--ink, #1A1714); color:#fff; border:none; border-radius:8px; cursor:pointer; font-size:1rem; font-weight:600; }
  `],
})
export class ConviteAceitarComponent implements OnInit {
  readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly conviteService = inject(ConviteService);

  loading = signal(true);
  erro = signal<string | null>(null);
  mensagemSucesso = signal('Convite aceito! Você agora é participante do cofre.');
  redirectPath = signal('/cofres');
  botaoTexto = signal('Ver meus cofres');

  ngOnInit(): void {
    const token = this.route.snapshot.queryParamMap.get('token');
    const tipo = this.route.snapshot.queryParamMap.get('tipo');

    if (!token) {
      this.erro.set('Token de convite inválido.');
      this.loading.set(false);
      return;
    }

    if (tipo === 'grupo') {
      this.mensagemSucesso.set('Convite aceito! Você agora é membro do grupo.');
      this.redirectPath.set('/grupos');
      this.botaoTexto.set('Ver meus grupos');

      this.conviteService.aceitarGrupo(token).subscribe({
        next: () => this.loading.set(false),
        error: (err) => {
          this.erro.set(err?.error?.errors?.[0] ?? 'Erro ao aceitar convite.');
          this.loading.set(false);
        },
      });
    } else {
      this.conviteService.aceitar(token).subscribe({
        next: () => this.loading.set(false),
        error: (err) => {
          this.erro.set(err?.error?.errors?.[0] ?? 'Erro ao aceitar convite.');
          this.loading.set(false);
        },
      });
    }
  }
}
