import { Routes } from '@angular/router';
import { LayoutComponent } from './shared/components/layout/layout.component';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  {
    path: 'login',
    loadComponent: () =>
      import('./features/auth/login/login.component').then((m) => m.LoginComponent),
  },
  {
    path: 'cadastro',
    loadComponent: () =>
      import('./features/auth/cadastro/cadastro.component').then((m) => m.CadastroComponent),
  },
  {
    path: 'aguardando-confirmacao',
    loadComponent: () =>
      import('./features/auth/aguardando-confirmacao/aguardando-confirmacao.component').then(
        (m) => m.AguardandoConfirmacaoComponent
      ),
  },
  {
    path: '',
    component: LayoutComponent,
    canActivate: [authGuard],
    children: [
      {
        path: 'home',
        loadComponent: () =>
          import('./features/home/home.component').then((m) => m.HomeComponent),
      },
      {
        path: 'cofres',
        loadChildren: () =>
          import('./features/cofres/cofres.routes').then((m) => m.cofresRoutes),
      },
      {
        path: 'despesas',
        loadComponent: () =>
          import('./features/despesas/despesas-list/despesas.component').then(
            (m) => m.DespesasComponent
          ),
      },
      {
        path: 'receitas',
        loadComponent: () =>
          import('./features/receita/receita-list/receita-list').then(
            (m) => m.ReceitaList
          ),
      },
      {
        path: 'convites/aceitar',
        loadComponent: () =>
          import('./features/convites/convite-aceitar/convite-aceitar.component').then(
            (m) => m.ConviteAceitarComponent
          ),
      },
    ],
  },
  { path: '**', redirectTo: 'login' },
];

