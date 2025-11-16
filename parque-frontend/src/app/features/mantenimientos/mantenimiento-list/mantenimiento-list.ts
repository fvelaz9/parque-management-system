// src/app/features/mantenimientos/mantenimiento-list/mantenimiento-list.ts
import { Component, inject, signal, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { MantenimientoPreventivo } from '../../../core/models/mantenimiento.model';
import { MantenimientosService } from '../../../core/services/mantenimiento.service';
import { AuthService } from '../../../core/services/auth.service';
import {TipoAtraccion} from '../../../core/models/atraccion.model';

@Component({
  selector: 'app-mantenimiento-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './mantenimiento-list.html',
  styleUrl: './mantenimiento-list.css'
})
export class MantenimientoList {
  private readonly mantenimientosService = inject(MantenimientosService);
  private readonly authService = inject(AuthService);  // ← AGREGAR
  private readonly router = inject(Router);

  public mantenimientos = signal<MantenimientoPreventivo[]>([]);
  public loading = signal(true);
  public error = signal('');

  private readonly loadMantenimientosEffect = effect(() => {
    this.cargarMantenimientos();
  });

  getNombreTipo(tipoId: TipoAtraccion): string {
    const tipo = this.tiposDisponibles.find(t => t.id === tipoId);
    return tipo ? tipo.nombre : 'Desconocido';
  }

  tiposDisponibles = [
    { id: TipoAtraccion.MontañaRusa, nombre: 'Montaña Rusa' },
    { id: TipoAtraccion.Simulador, nombre: 'Simulador' },
    { id: TipoAtraccion.Espectaculo, nombre: 'Espectaculo' },
    { id: TipoAtraccion.ZonaInteractiva, nombre: 'Zona Interactiva' }
  ];
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

  // ← AGREGAR ESTOS GETTERS (igual que recompensas)
  get isAdmin(): boolean {
    return this.authService.getUsuario()?.roles?.includes('Administrador') || false;
  }

  get isOperador(): boolean {
    return this.authService.getUsuario()?.roles?.includes('Operador') || false;
  }

  get puedeGestionar(): boolean {
    return this.isAdmin || this.isOperador;
  }

  eliminarMantenimiento(id: number) {
    if (confirm('¿Estás seguro de que deseas eliminar este mantenimiento?')) {
      this.mantenimientosService.deleteMantenimiento(id).subscribe({
        next: () => {
          console.log('Mantenimiento eliminado');
          this.cargarMantenimientos();
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
