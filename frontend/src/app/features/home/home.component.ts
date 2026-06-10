import { Component, inject, OnInit, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { CofreService } from '../../core/services/cofre.service';
import { ConviteService, ConviteResponse } from '../../core/services/convite.service';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RouterLink, DatePipe],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
})
export class HomeComponent implements OnInit {
  private readonly cofreService = inject(CofreService);
  private readonly conviteService = inject(ConviteService);
  private readonly authService = inject(AuthService);

  cofresAtivos = signal(0);
  totalParticipantes = signal(0);
  totalInvestido = signal(0);
  loading = signal(true);
  convitesPendentes = signal<ConviteResponse[]>([]);
  aceitando = signal<string | null>(null);
  recusando = signal<string | null>(null);

  ngOnInit(): void {
    this.cofreService.listar().subscribe({
      next: (cofres) => {
        this.cofresAtivos.set(cofres.filter(c => c.status.toLowerCase() === 'ativo').length);
        const meuId = this.authService.currentUser()?.usuarioId;
        const participantesConvidados = new Set(
          cofres.flatMap(c => c.participantes
            .filter(p => p.usuarioId !== meuId)
            .map(p => p.usuarioId)
          )
        );
        this.totalParticipantes.set(participantesConvidados.size);
        this.totalInvestido.set(cofres.reduce((acc, c) => acc + (c.totalMovimentado ?? 0), 0));
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
    this.carregarConvites();
  }

  carregarConvites(): void {
    this.conviteService.pendentes().subscribe({
      next: (list) => this.convitesPendentes.set(list),
    });
  }

  aceitar(token: string): void {
    this.aceitando.set(token);
    this.conviteService.aceitar(token).subscribe({
      next: () => { this.aceitando.set(null); this.carregarConvites(); },
      error: () => this.aceitando.set(null),
    });
  }

  recusar(token: string): void {
    this.recusando.set(token);
    this.conviteService.recusar(token).subscribe({
      next: () => { this.recusando.set(null); this.carregarConvites(); },
      error: () => this.recusando.set(null),
    });
  }

  formatarMoeda(valor: number): string {
    return valor.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
  }
}
