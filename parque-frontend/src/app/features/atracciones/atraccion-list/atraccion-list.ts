import { Component, inject, signal, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AtraccionParque } from '../../../core/models/atraccion.model';
import { AtraccionesService } from '../../../core/services/atracciones.service';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-atraccion-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './atraccion-list.html',
  styleUrl: './atraccion-list.css'
})
export class AtraccionListComponent {
  private readonly atraccionesService = inject(AtraccionesService);

  public atracciones = signal<AtraccionParque[]>([]);
  public loading = signal<boolean>(true);
  public error = signal<string>('');

  private readonly loadAtraccionesEffect = effect(() => {
    this.cargarAtracciones();
  });

  private cargarAtracciones() {
    this.loading.set(true);
    this.atraccionesService.getAllAtracciones().subscribe({
      next: (result) => {
        console.log('Atracciones cargadas:', result);
        this.atracciones.set(result);
        this.loading.set(false);
        this.error.set('');
      },
      error: (err) => {
        console.error('Error al cargar atracciones:', err);
        this.error.set('No se pudieron cargar las atracciones. Verifica que el backend esté corriendo.');
        this.loading.set(false);
      }
    });
  }

  editarAtraccion(atraccion: any) {
    this.atraccionesService.updateAtraccion(atraccion.id, atraccion).subscribe({
      next: (res) => {
        alert('Atracción modificada con éxito');
        this.cargarAtracciones();
      },
    });
  }

  borrarAtraccion(atraccion: any) {
    if (confirm(`¿Seguro que deseas borrar "${atraccion.nombre}"?`)) {
      this.atraccionesService.deleteAtraccion(atraccion.id).subscribe({
        next: (res) => {
          this.cargarAtracciones();
        },
        error: (err) => {
          alert('Error al borrar atracción');
        }
      });
    }
  }
}
