import { Routes } from '@angular/router';

export const gruposRoutes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./grupos-list/grupos-list.component').then((m) => m.GruposListComponent),
  },
  {
    path: ':id',
    loadComponent: () =>
      import('./grupo-detalhe/grupo-detalhe.component').then((m) => m.GrupoDetalheComponent),
  },
];
