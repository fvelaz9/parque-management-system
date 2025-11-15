import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { EventoService } from '../../../core/services/evento.service';
import { AtraccionesService } from '../../../core/services/atracciones.service';
import { EstadoEvento } from '../../../core/models/evento.model';
import {Router, RouterLink} from '@angular/router';
import {NgIf, NgForOf} from '@angular/common';

@Component({
  selector: 'app-evento-form',
  standalone: true,
  imports: [FormsModule, NgIf, NgForOf, RouterLink],
  templateUrl: './evento-form.html',
  styleUrls: ['./evento-form.css'],
})
export class EventoForm {
  evento = {
    titulo: '',
    descripcion: '',
    inicio: '',
    fin: '',
    aforoMaximo: 0,
    costoAdicional: 0,
    estado: EstadoEvento.Programado,
    atraccionIds: [] as number[],
  };

  estadoEnum = EstadoEvento;

  atraccionesDisponibles: { id: number; nombre: string }[] = [];

  private readonly eventoService = inject(EventoService);
  private readonly atraccionesService = inject(AtraccionesService);
  private readonly router = inject(Router);

  constructor() {
    this.cargarAtracciones();
  }

  cargarAtracciones() {
    this.atraccionesService.getAllAtracciones().subscribe({
      next: (result) => {
        this.atraccionesDisponibles = result;
      },
    });
  }

  onSubmit() {
    this.eventoService.crearEvento(this.evento).subscribe({
      next: () => {
        alert('Evento creado con éxito');
        this.router.navigate(['/eventos']);
      },
      error: () => {
        alert('Error al crear evento');
      }
    });
  }
  onToggleAtraccion(id: number, checked: boolean) {
    if (checked) {
      this.evento.atraccionIds.push(id);
    } else {
      this.evento.atraccionIds = this.evento.atraccionIds.filter(a => a !== id);
    }
  }
}
