import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { EventoService } from '../../../core/services/evento.service';
import { Evento } from '../../../core/models/evento.model';
import {AtraccionesService} from '../../../core/services/atracciones.service';
import {AtraccionParque} from '../../../core/models/atraccion.model';

@Component({
  selector: 'app-evento-atracciones',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './evento-atracciones.html',
  styleUrl: './evento-atracciones.css',
})
export class EventoAtracciones {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private eventoService = inject(EventoService);
  private atraccionService = inject(AtraccionesService);
  public evento = signal<Evento | null>(null);
  public atracciones = signal<AtraccionParque[]>([]);
  public loading = signal<boolean>(true);
  public errorMessage = signal<string>('');

  constructor() {
    this.route.queryParams.subscribe(params => {
      const eventoId = +params['eventoId'] || 0;
      if (eventoId) {
        this.cargarEventoYAtracciones(eventoId);
      } else {
        alert('No se proporcionó eventoId');
        this.router.navigate(['/eventos']);
      }
    });
  }

  private cargarEventoYAtracciones(eventoId: number) {
    this.loading.set(true);
    this.eventoService.listarEventos().subscribe({
      next: (eventos) => {
        const eventoEncontrado = eventos.find(e => e.id === eventoId);
        if (eventoEncontrado) {
          this.evento.set(eventoEncontrado);
          this.cargarAtraccionesDelEvento(eventoId);
          this.errorMessage.set('');
        } else {
          alert('Evento no encontrado');
          this.router.navigate(['/eventos']);
        }
      },
      error: () => {
        alert('Error al cargar evento');
        this.router.navigate(['/eventos']);
      }
    });
  }

  private cargarAtraccionesDelEvento(eventoId: number) {
    this.eventoService.obtenerAtraccionesPorEvento(eventoId).subscribe({
      next: (atracciones) => {
        this.atracciones.set(atracciones);
        this.loading.set(false);
      },
      error: (error) => {
        console.error('Error al cargar atracciones del evento:', error);
        this.errorMessage = error.error?.message || 'Error al cargar atracciones del evento';
        this.loading.set(false);
      }
    });
  }

  irAAcceso(atraccionId: number) {
    this.router.navigate(['/acceso-ingreso'], {
      queryParams: {
        atraccionId: atraccionId,
        eventoId: this.evento()?.id
      }
    });
  }

  volver() {
    this.router.navigate(['/eventos']);
  }
}
