// src/app/features/reporte-uso/reporte-uso.component.ts
import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment.development';

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
export class ReporteUsoComponent {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = environment.apiUrl;

  public reporteAtracciones = signal<ReporteAtraccion[]>([]);
  public loading = signal(false);
  public error = signal('');
  public fechaDesde: string = new Date(Date.now() - 7 * 24 * 60 * 60 * 1000).toISOString().split('T')[0];
  public fechaHasta: string = new Date().toISOString().split('T')[0];

  cargarReporte() {
    if (!this.fechaDesde || !this.fechaHasta) {
      this.error.set('Debe seleccionar ambas fechas');
      return;
    }

    if (new Date(this.fechaDesde) > new Date(this.fechaHasta)) {
      this.error.set('La fecha desde no puede ser mayor a la fecha hasta');
      return;
    }

    this.loading.set(true);
    this.error.set('');

    const url = `${this.apiUrl}/atracciones/reporte-uso?desde=${this.fechaDesde}&hasta=${this.fechaHasta}`;

    this.http.get<ReporteAtraccion[]>(url).subscribe({
      next: (response) => {
        console.log('Reporte response:', response);
        this.reporteAtracciones.set(response);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error al cargar reporte:', err);
        this.error.set('Error al cargar el reporte de uso');
        this.loading.set(false);
      }
    });
  }
}
