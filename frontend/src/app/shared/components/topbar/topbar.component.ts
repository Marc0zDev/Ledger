import { Component, inject, computed } from '@angular/core';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-topbar',
  standalone: true,
  imports: [],
  templateUrl: './topbar.component.html',
  styleUrl: './topbar.component.scss',
})
export class TopbarComponent {
  private readonly authService = inject(AuthService);

  readonly userName    = computed(() => this.authService.currentUser()?.nome ?? '');
  readonly userInitial = computed(() => this.userName()[0]?.toUpperCase() ?? '?');

  logout(): void {
    this.authService.logout();
  }
}
