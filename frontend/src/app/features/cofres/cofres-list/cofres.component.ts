import { Component, inject, OnInit, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { CofreService } from '../../../core/services/cofre.service';
import { CofreResponse } from '../../../core/models/cofre.model';

@Component({
  selector: 'app-cofres',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './cofres.component.html',
  styleUrl: './cofres.component.scss',
})
export class CofresComponent implements OnInit {
  private readonly cofreService = inject(CofreService);
  private readonly router = inject(Router);

  cofres = signal<CofreResponse[]>([]);
  loading = signal(true);
  erro = signal<string | null>(null);

  ngOnInit(): void {
    this.cofreService.listar().subscribe({
      next: (data) => {
        this.cofres.set(data);
        this.loading.set(false);
      },
      error: () => {
        this.erro.set('Não foi possível carregar os cofres.');
        this.loading.set(false);
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

  abrirCofre(id: string): void {
    this.router.navigate(['/cofres', id]);
  }
}
