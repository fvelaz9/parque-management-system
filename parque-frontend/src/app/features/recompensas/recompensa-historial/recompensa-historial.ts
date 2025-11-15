// src/app/features/recompensas/historial-canje/historial-canje.component.ts
import { Component, effect, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { RecompensasService } from '../../../core/services/recompensa.service';
import { AuthService } from '../../../core/services/auth.service';
import { HistorialCanjeDto } from '../../../core/models/recompensa.model';

@Component({
  selector: 'app-historial-canje',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './recompensa-historial.html',
  styleUrl: './recompensa-historial.css'
})
export class HistorialCanje {
  private readonly recompensasService = inject(RecompensasService);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  historial = signal<HistorialCanjeDto[]>([]);
  totalCanjes = signal(0);
  loading = signal(true);
  error = signal('');

  private readonly loadEffect = effect(() => {
    this.cargarHistorial();
  });

  private cargarHistorial() {
    const visitanteId = this.visitanteId;

    if (!visitanteId) {
      this.error.set('No se pudo obtener el ID del visitante');
      this.loading.set(false);
      return;
    }

    this.loading.set(true);
    this.error.set('');

    this.recompensasService.getHistorial(visitanteId).subscribe({
      next: (resp) => {
        this.historial.set(resp.historial);
        this.totalCanjes.set(resp.totalCanjes);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error al cargar historial:', err);
        this.error.set('Error al cargar el historial de canjes');
        this.loading.set(false);
      }
    });
  }

  get visitanteId(): string | null {
    const usuario = this.authService.getUsuario();
    return usuario?.visitante?.id || null;
  }

  volver() {
    this.router.navigate(['/recompensas']);
  }
}
