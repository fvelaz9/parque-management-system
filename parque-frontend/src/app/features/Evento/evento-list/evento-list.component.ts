import { Component, inject, signal, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import {EstadoEvento, Evento} from '../../../core/models/evento.model';
import { EventoService } from '../../../core/services/evento.service';
import { Router } from '@angular/router';
import {AuthService} from '../../../core/services/auth.service';

@Component({
  selector: 'app-evento-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './evento-list.component.html',
  styleUrls: ['./evento-list.component.css']
})
export class EventoListComponent {
  private readonly eventoService = inject(EventoService);
  private readonly authService = inject(AuthService);
  public eventos = signal<Evento[]>([]);
  public loading = signal<boolean>(true);
  public error = signal<string>('');
  private readonly router = inject(Router);

  private readonly loadEventosEffect = effect(() => {
    this.cargarEventos();
  });

  private cargarEventos() {
    this.loading.set(true);
    this.eventoService.listarEventos().subscribe({
      next: (result) => {
        this.eventos.set(result);
        this.loading.set(false);
        this.error.set('');
      },
      error: (err) => {
        console.error('Error al cargar eventos:', err);
        this.error.set('No se pudieron cargar los eventos. Verifica que el backend esté corriendo.');
        this.loading.set(false);
      }
    });
  }

  eliminarEvento(evento: Evento) {
    console.log('ID evento a eliminar:', evento.id);
    if (!evento.id) {
      alert('ID de evento inválido.');
      return;
    }
    if (confirm(`¿Seguro que deseas borrar "${evento.titulo}"?`)) {
      this.eventoService.deleteEvento(evento.id).subscribe({
        next: () => {
          this.cargarEventos();
        },
        error: () => {
          alert('Error al borrar evento');
        }
      });
    }
  }
  verAtracciones(evento: Evento) {
    if (!evento.id) {
      alert('Evento sin ID válido');
      return;
    }
    this.router.navigate(['/evento-atracciones'], {
      queryParams: { eventoId: evento.id }
    });
  }

  agregarEvento() {
    this.router.navigate(['/eventos/nuevo']);
  }
  protected readonly EstadoEvento = EstadoEvento;

  get isAdmin(): boolean {
    return this.authService.getUsuario()?.roles?.includes('Administrador') || false;
  }
}
