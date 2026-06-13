import { Component, signal } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { MenuItem } from './sidebar-menu-item.model';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss',
})
export class SidebarComponent {
  collapsed = signal(false);
  toggle() { this.collapsed.update(v => !v); }

  menuItems = signal<MenuItem[]>([
    { label: 'Home', route: '/home', icon: 'home' },
    { label: 'Cofres', route: '/cofres', icon: 'account_balance' },
    { label: 'Despesas', route: '/despesas', icon: 'wallet' },
    { label: 'Receitas', route: '/receitas', icon: 'attach_money' },
  ]);
}
