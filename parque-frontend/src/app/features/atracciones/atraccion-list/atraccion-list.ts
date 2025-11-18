import {Component, effect, inject, signal} from '@angular/core';
import {CommonModule} from '@angular/common';
import {AtraccionParque, EstadoAtraccion, TipoAtraccion} from '../../../core/models/atraccion.model';
import {AtraccionesService} from '../../../core/services/atracciones.service';
import {Router, RouterLink} from '@angular/router';
import {AuthService} from '../../../core/services/auth.service';

@Component({
  selector: 'app-atraccion-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './atraccion-list.html',
  styleUrl: './atraccion-list.css'
})
export class AtraccionListComponent {
  private readonly atraccionesService = inject(AtraccionesService);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  public atracciones = signal<AtraccionParque[]>([]);
  public loading = signal<boolean>(true);
  public error = signal<string>('');

  private readonly loadAtraccionesEffect = effect(() => {
    this.cargarAtracciones();
  });

  estadosDisponibles = [
    { id: EstadoAtraccion.Disponible, nombre: 'Disponible' },
    { id: EstadoAtraccion.FueraDeServicio, nombre: 'Fuera De Servicio' }
  ];
  getEstadoNombre(estadoId: EstadoAtraccion): string {
    const tipo = this.estadosDisponibles.find(t => t.id === estadoId);
    return tipo ? tipo.nombre : 'Desconocido';
  }
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

  private cargarAtracciones() {
    this.loading.set(true);
    this.atraccionesService.getAllAtracciones().subscribe({
      next: (result) => {
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

  get isAdmin(): boolean {
    return this.authService.getUsuario()?.roles?.includes('Administrador') || false;
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
