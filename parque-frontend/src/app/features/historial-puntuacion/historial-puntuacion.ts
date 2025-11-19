// src/app/features/historial-puntuacion/historial-puntuacion.ts

import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { GamificacionService } from '../../core/services/gamificacion.service';
import { AuthService } from '../../core/services/auth.service';
import { HistorialPuntuacionDto } from '../../core/models/gamificacion.model';

@Component({
  selector: 'app-historial-puntuacion',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './historial-puntuacion.html',
  styleUrl: './historial-puntuacion.css'
})
export class HistorialPuntuacionComponent implements OnInit {
  private readonly gamificacionService = inject(GamificacionService);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  public historial = signal<HistorialPuntuacionDto[]>([]);
  public loading = signal(false);
  public error = signal('');

  ngOnInit(): void {
    const usuario = this.authService.getUsuario();

    if (!usuario?.visitante?.id) {
      this.error.set('No se encontró información del visitante. Por favor, inicie sesión nuevamente.');
      return;
    }

    this.cargarHistorial(usuario.visitante.id);
  }

  private cargarHistorial(visitanteId: string): void {
    this.loading.set(true);
    this.error.set('');

    this.gamificacionService.getHistorialVisitante(visitanteId)
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
            this.error.set('No se encontró historial para este visitante');
          } else {
            this.error.set(err.error?.message || 'Error al cargar el historial');
          }

          this.loading.set(false);
        }
      });
  }
}
