// src/app/features/historial-puntuacion/historial-puntuacion.ts

import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { GamificacionService } from '../../core/services/gamificacion.service';
import { AuthService } from '../../core/services/auth.service';
import { HistorialPuntuacionDto } from '../../core/models/gamificacion.model';

@Component({
  selector: 'app-historial-puntuacion',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './historial-puntuacion.html',
  styleUrl: './historial-puntuacion.css'
})
export class HistorialPuntuacionComponent {
  private readonly gamificacionService = inject(GamificacionService);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  public historial = signal<HistorialPuntuacionDto[]>([]);
  public loading = signal(false);
  public error = signal('');

  public visitanteId: string = '';

  buscarHistorial(): void {
    this.cargarHistorial();
  }

  private cargarHistorial(): void {
    if (!this.visitanteId) {
      this.error.set('Debe ingresar un ID de visitante');
      return;
    }

    this.loading.set(true);
    this.error.set('');

    this.gamificacionService.getHistorialVisitante(this.visitanteId)
      .subscribe({
        next: (response) => {
          if (response.executionSuccessful) {
            this.historial.set(response.content || []);
          } else {
            this.error.set(response.message || 'Error al cargar el historial');
          }
          this.loading.set(false);
        },
        error: (err) => {
          console.error('Error al cargar historial:', err);

          if (err.status === 401) {
            this.error.set('No estás autenticado');
            this.router.navigate(['/login']);
          } else if (err.status === 403) {
            this.error.set('No tienes permisos para ver el historial');
          } else if (err.status === 404) {
            this.error.set('Visitante no encontrado');
          } else {
            this.error.set(err.error?.message || 'Error al cargar el historial');
          }

          this.loading.set(false);
        }
      });
  }
}
