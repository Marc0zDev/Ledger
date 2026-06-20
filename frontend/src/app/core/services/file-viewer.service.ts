import { Injectable, inject, signal } from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { ArquivoService } from './arquivo.service';
import { NotificationService } from './notification.service';

@Injectable({ providedIn: 'root' })
export class FileViewerService {
  private readonly arquivoService = inject(ArquivoService);
  private readonly notify         = inject(NotificationService);
  private readonly sanitizer      = inject(DomSanitizer);

  readonly visible   = signal(false);
  readonly safeUrl   = signal<SafeResourceUrl | null>(null);
  readonly mimeType  = signal<string>('application/pdf');
  readonly loading   = signal(false);

  private currentUrl: string | null = null;

  open(arquivoId: string): void {
    if (this.loading()) return;
    this.loading.set(true);

    this.arquivoService.obterBlob(arquivoId).subscribe({
      next: (blob) => {
        this.revogarUrl();
        const url = URL.createObjectURL(blob);
        this.currentUrl = url;
        this.mimeType.set(blob.type || 'application/pdf');
        this.safeUrl.set(this.sanitizer.bypassSecurityTrustResourceUrl(url));
        this.visible.set(true);
        this.loading.set(false);
      },
      error: () => {
        this.notify.error('Não foi possível abrir o arquivo.');
        this.loading.set(false);
      },
    });
  }

  close(): void {
    this.visible.set(false);
    this.safeUrl.set(null);
    this.revogarUrl();
  }

  private revogarUrl(): void {
    if (this.currentUrl) {
      URL.revokeObjectURL(this.currentUrl);
      this.currentUrl = null;
    }
  }
}
