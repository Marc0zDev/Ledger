import { Routes } from '@angular/router';

export const cofresRoutes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./cofres-list/cofres.component').then((m) => m.CofresComponent),
  },
  {
    path: 'novo',
    loadComponent: () =>
      import('./criar-cofre/criar-cofre.component').then((m) => m.CriarCofreComponent),
  },
  {
    path: ':id',
    loadComponent: () =>
      import('./cofre-detalhe/cofre-detalhe.component').then((m) => m.CofreDetalheComponent),
  },
];
