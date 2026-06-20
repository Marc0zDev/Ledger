import { Component, inject } from '@angular/core';
import { FileViewerService } from '../../../core/services/file-viewer.service';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { ProgressSpinnerModule } from 'primeng/progressspinner';

@Component({
  selector: 'app-file-viewer',
  standalone: true,
  imports: [DialogModule, ButtonModule, ProgressSpinnerModule],
  templateUrl: './file-viewer.component.html',
  styleUrl: './file-viewer.component.scss',
})
export class FileViewerComponent {
  readonly viewer = inject(FileViewerService);

  get isImage(): boolean {
    return this.viewer.mimeType().startsWith('image/');
  }

  onHide(): void {
    this.viewer.close();
  }
}
