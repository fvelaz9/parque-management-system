import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment.development';

interface Atraccion {
  id: number;
  nombre: string;
  tipo: number;
  edadMinima: number;
  capacidad: number;
  descripcion: string;
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
export class ReportesComponent implements OnInit {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = environment.apiUrl;

  // Estados para lista de atracciones
  public atracciones = signal<Atraccion[]>([]);
  public loading = signal(true);

  // Estados para reporte
  public reporte = signal<ReporteAtraccion | null>(null);
  public submitting = signal(false);
  public error = signal('');
  public successMessage = signal('');

  // Filtros
  public atraccionSeleccionadaId: number = 0;
  public fechaDesde: string = new Date(Date.now() - 7 * 24 * 60 * 60 * 1000).toISOString().split('T')[0];
  public fechaHasta: string = new Date().toISOString().split('T')[0];

  ngOnInit() {
    this.cargarAtracciones();
  }

  private cargarAtracciones() {
    this.loading.set(true);
    const url = `${this.apiUrl}/atracciones`;

    this.http.get<Atraccion[]>(url).subscribe({
      next: (result) => {
        console.log('Atracciones cargadas:', result);
        this.atracciones.set(result);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error al cargar atracciones:', err);
        alert('Error al cargar las atracciones');
        this.loading.set(false);
      }
    });
  }

  cargarReporte() {
    if (!this.validarFormulario()) {
      return;
    }

    this.submitting.set(true);
    this.error.set('');
    this.successMessage.set('');

    const url = `${this.apiUrl}/atracciones/reporte-uso?atraccionId=${this.atraccionSeleccionadaId}&desde=${this.fechaDesde}&hasta=${this.fechaHasta}`;

    this.http.get<ReporteAtraccion>(url).subscribe({
      next: (response) => {
        console.log('Reporte response:', response);
        this.reporte.set(response);
        this.successMessage.set(`Reporte generado correctamente para ${response.nombreAtraccion}`);
        this.submitting.set(false);
      },
      error: (err) => {
        console.error('Error al cargar reporte:', err);
        const mensaje = err.error?.mensaje || 'Error al cargar el reporte de uso';
        alert(mensaje);
        this.error.set(mensaje);
        this.submitting.set(false);
      }
    });
  }

  private validarFormulario(): boolean {
    if (this.atraccionSeleccionadaId === 0) {
      alert('Debe seleccionar una atracción');
      return false;
    }

    if (!this.fechaDesde || !this.fechaHasta) {
      alert('Debe seleccionar ambas fechas');
      return false;
    }

    if (new Date(this.fechaDesde) > new Date(this.fechaHasta)) {
      alert('La fecha desde no puede ser mayor a la fecha hasta');
      return false;
    }

    return true;
  }
}
