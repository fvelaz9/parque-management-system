// src/app/features/mantenimientos/mantenimiento-list/mantenimiento-list.ts
import { Component, inject, signal, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { MantenimientoPreventivo } from '../../../core/models/mantenimiento.model';
import { MantenimientosService } from '../../../core/services/mantenimiento.service';

@Component({
  selector: 'app-mantenimiento-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './mantenimiento-list.html',
  styleUrl: './mantenimiento-list.css'
})
export class MantenimientoList {
  private readonly mantenimientosService = inject(MantenimientosService);
  private readonly router = inject(Router);

  public mantenimientos = signal<MantenimientoPreventivo[]>([]);
  public loading = signal(true);
  public error = signal('');

  private readonly loadMantenimientosEffect = effect(() => {
    this.cargarMantenimientos();
  });

  private cargarMantenimientos() {
    this.loading.set(true);
    this.mantenimientosService.getAllMantenimientos().subscribe({
      next: (result) => {
        console.log('Mantenimientos cargados:', result);
        this.mantenimientos.set(result);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error al cargar mantenimientos:', err);
        this.error.set('Error al cargar los mantenimientos');
        this.loading.set(false);
      }
    });
  }

  eliminarMantenimiento(id: number) {
    if (confirm('¿Estás seguro de que deseas eliminar este mantenimiento?')) {
      this.mantenimientosService.deleteMantenimiento(id).subscribe({
        next: () => {
          console.log('Mantenimiento eliminado');
          this.cargarMantenimientos(); // Recargar lista
        },
        error: (err) => {
          console.error('Error al eliminar mantenimiento:', err);
          alert('Error al eliminar el mantenimiento');
        }
      });
    }
  }
  editarMantenimiento(id: number) {
    this.router.navigate(['/mantenimientos/editar', id]);
  }

  formatearDuracion(duracion: string): string {
    const parts = duracion.split(':');
    return `${parts[0]}h ${parts[1]}m`;
  }
}
