// src/app/features/ranking/ranking.ts

import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { RankingVisitanteDto } from '../../core/models/gamificacion.model';
import { GamificacionService } from '../../core/services/gamificacion.service';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-ranking',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './ranking.html',
  styleUrl: './ranking.css'
})
export class RankingComponent {
  private readonly gamificacionService = inject(GamificacionService);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  public rankingVisitantes = signal<RankingVisitanteDto[]>([]);
  public loading = signal(false);
  public error = signal('');

  // Filtros
  public fechaRanking: string = this.formatDate(new Date());
  public topRanking: number = 10;

  cargarRanking(): void {
    this.loading.set(true);
    this.error.set('');

    this.gamificacionService.getRankingDiario(this.fechaRanking, this.topRanking)
      .subscribe({
        next: (response) => {
          if (response.executionSuccessful) {
            const ranking = response.content.ranking || [];
            this.rankingVisitantes.set(ranking);

            if (ranking.length === 0) {
              this.error.set('No hay visitantes en el ranking para la fecha seleccionada');
            }
          } else {
            this.error.set(response.message || 'Error al cargar el ranking');
          }
          this.loading.set(false);
        },
        error: (err) => {
          console.error('Error al cargar ranking:', err);

          if (err.status === 401) {
            this.error.set('No estás autenticado');
            this.router.navigate(['/login']);
          } else if (err.status === 403) {
            this.error.set('No tienes permisos para ver el ranking');
          } else {
            this.error.set(err.error?.message || 'Error al cargar el ranking');
          }

          this.loading.set(false);
        }
      });
  }

  // Formatea fecha a yyyy-MM-dd para el input type="date"
  private formatDate(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }
}
