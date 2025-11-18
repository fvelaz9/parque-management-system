import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EstrategiasService } from '../../../core/services/estrategias.service';
import { EstrategiaInfo } from '../../../core/models/estrategia.model';

@Component({
  selector: 'app-selector-estrategias',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './selector-estrategias.html',
  styleUrls: ['./selector-estrategias.css']
})
export class SelectorEstrategiasComponent implements OnInit {
  private estrategiasService = inject(EstrategiasService);

  estrategias: EstrategiaInfo[] = [];
  estrategiaSeleccionada: string = '';
  loading = false;
  errorMessage = '';
  successMessage = '';
  loadingRecargar = false;

  ngOnInit(): void {
    this.cargarEstrategias();
  }

  cargarEstrategias(): void {
    this.loading = true;
    this.errorMessage = '';
    
    this.estrategiasService.obtenerEstrategiasDisponibles().subscribe({
      next: (response) => {
        if (response.executionSuccessful) {
          this.estrategias = response.content;
          
          // Seleccionar la estrategia activa actual
          const activa = this.estrategias.find(e => e.esActiva);
          if (activa) {
            this.estrategiaSeleccionada = activa.nombre;
          }
        } else {
          this.errorMessage = response.message || 'Error al cargar estrategias';
        }
        
        this.loading = false;
      },
      error: (error) => {
        console.error('Error al cargar estrategias:', error);
        this.errorMessage = error.error?.message || 'Error al cargar estrategias';
        this.loading = false;
      }
    });
  }

  cambiarEstrategia(): void {
    if (!this.estrategiaSeleccionada) return;

    // No cambiar si ya está activa
    if (this.estrategiaActual?.nombre === this.estrategiaSeleccionada) {
      this.successMessage = 'Esta estrategia ya está activa';
      return;
    }

    this.loading = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.estrategiasService.cambiarEstrategiaActiva(this.estrategiaSeleccionada).subscribe({
      next: (response) => {
        if (response.executionSuccessful) {
          this.successMessage = response.message || 'Estrategia cambiada exitosamente';
          this.cargarEstrategias(); // Recargar para actualizar estado
        } else {
          this.errorMessage = response.message || 'Error al cambiar estrategia';
          this.loading = false;
        }
      },
      error: (error) => {
        console.error('Error al cambiar estrategia:', error);
        this.errorMessage = error.error?.message || 'Error al cambiar estrategia';
        this.loading = false;
      }
    });
  }

  recargarPlugins(): void {
    this.loadingRecargar = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.estrategiasService.recargarPlugins().subscribe({
      next: (response) => {
        if (response.executionSuccessful) {
          this.estrategias = response.content;
          this.successMessage = response.message || 'Plugins recargados exitosamente';
          
          // Mantener selección si aún existe
          const existeSeleccion = this.estrategias.some(e => e.nombre === this.estrategiaSeleccionada);
          if (!existeSeleccion) {
            const activa = this.estrategias.find(e => e.esActiva);
            this.estrategiaSeleccionada = activa?.nombre || '';
          }
        } else {
          this.errorMessage = response.message || 'Error al recargar plugins';
        }
        
        this.loadingRecargar = false;
      },
      error: (error) => {
        console.error('Error al recargar plugins:', error);
        this.errorMessage = error.error?.message || 'Error al recargar plugins';
        this.loadingRecargar = false;
      }
    });
  }

  limpiarMensajes(): void {
    this.errorMessage = '';
    this.successMessage = '';
  }

  get estrategiaActual(): EstrategiaInfo | undefined {
    return this.estrategias.find(e => e.esActiva);
  }

  get cantidadBase(): number {
    return this.estrategias.filter(e => e.origen === 'Base').length;
  }

  get cantidadPlugins(): number {
    return this.estrategias.filter(e => e.origen === 'Plugin').length;
  }

  get estrategiasBase(): EstrategiaInfo[] {
    return this.estrategias.filter(e => e.origen === 'Base');
  }

  get estrategiasPlugin(): EstrategiaInfo[] {
    return this.estrategias.filter(e => e.origen === 'Plugin');
  }
}