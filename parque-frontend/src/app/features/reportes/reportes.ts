// src/app/features/reportes/reportes.component.ts
import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment.development';

interface RankingVisitante {
  visitanteId: string;
  nombreVisitante: string;
  puntosTotales: number;
  posicion: number;
}

interface ReporteAtraccion {
  atraccionId: number;
  nombreAtraccion: string;
  cantidadVisitas: number;
}

@Component({
  selector: 'app-reportes',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './reportes.html',
  styleUrl: './reportes.css'
})
export class ReportesComponent {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = environment.apiUrl;

  // Estados para Ranking Diario
  public rankingVisitantes = signal<RankingVisitante[]>([]);
  public loadingRanking = signal(false);
  public errorRanking = signal('');
  public fechaRanking: string = new Date().toISOString().split('T')[0];
  public topRanking: number = 10;

  // Estados para Reporte de Atracciones
  public reporteAtracciones = signal<ReporteAtraccion[]>([]);
  public loadingReporte = signal(false);
  public errorReporte = signal('');
  public fechaDesde: string = new Date(Date.now() - 7 * 24 * 60 * 60 * 1000).toISOString().split('T')[0];
  public fechaHasta: string = new Date().toISOString().split('T')[0];

  // Cargar Ranking Diario
  cargarRankingDiario() {
    this.loadingRanking.set(true);
    this.errorRanking.set('');

    const fecha = this.fechaRanking || new Date().toISOString().split('T')[0];
    const url = `${this.apiUrl}/gamificacion/ranking/diario?fecha=${fecha}&top=${this.topRanking}`;

    this.http.get<any>(url).subscribe({
      next: (response) => {
        console.log('Ranking response:', response);
        const ranking = response.content?.ranking || response.ranking || [];
        this.rankingVisitantes.set(ranking);
        this.loadingRanking.set(false);
      },
      error: (err) => {
        console.error('Error al cargar ranking:', err);
        this.errorRanking.set('Error al cargar el ranking diario');
        this.loadingRanking.set(false);
      }
    });
  }

  // Cargar Reporte de Atracciones
  cargarReporteAtracciones() {
    if (!this.fechaDesde || !this.fechaHasta) {
      this.errorReporte.set('Debe seleccionar ambas fechas');
      return;
    }

    if (new Date(this.fechaDesde) > new Date(this.fechaHasta)) {
      this.errorReporte.set('La fecha desde no puede ser mayor a la fecha hasta');
      return;
    }

    this.loadingReporte.set(true);
    this.errorReporte.set('');

    const url = `${this.apiUrl}/atracciones/reporte-uso?desde=${this.fechaDesde}&hasta=${this.fechaHasta}`;

    this.http.get<ReporteAtraccion[]>(url).subscribe({
      next: (response) => {
        console.log('Reporte response:', response);
        this.reporteAtracciones.set(response);
        this.loadingReporte.set(false);
      },
      error: (err) => {
        console.error('Error al cargar reporte:', err);
        this.errorReporte.set('Error al cargar el reporte de uso');
        this.loadingReporte.set(false);
      }
    });
  }
}
