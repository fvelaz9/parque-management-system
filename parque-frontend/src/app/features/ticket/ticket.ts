import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TicketService } from '../../core/services/ticket.service';
import { EventoService } from '../../core/services/evento.service';
import { CrearTicketDto, TipoTicket, Ticket } from '../../core/models/ticket.model';
import { Evento } from '../../core/models/evento.model';

@Component({
  selector: 'app-ticket',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './ticket.html',
  styleUrls: ['./ticket.css']
})
export class ComprarTicketComponent implements OnInit {
  private ticketService = inject(TicketService);
  private eventoService = inject(EventoService);

  fechaVisita: string = '';
  tipoEntrada: TipoTicket = TipoTicket.General;
  eventoSeleccionado?: number;
  eventos: Evento[] = [];
  loading = false;
  error: string = '';
  success: string = '';
  ticketCreado?: Ticket;

  // Exponer el enum al template
  TipoTicket = TipoTicket;

  // Fecha mínima (hoy)
  get fechaMinima(): string {
    return new Date().toISOString().split('T')[0];
  }

  ngOnInit(): void {
    this.cargarEventos();
  }

  cargarEventos(): void {
    this.eventoService.listarEventos().subscribe({
      next: (eventos) => {
        this.eventos = eventos;
      },
      error: (err) => {
        console.error('Error al cargar eventos:', err);
        this.error = 'No se pudieron cargar los eventos disponibles';
      }
    });
  }

  onTipoEntradaChange(): void {
    if (this.tipoEntrada === TipoTicket.General) {
      this.eventoSeleccionado = undefined;
    }
  }

  comprarTicket(): void {
    this.error = '';
    this.success = '';

    if (!this.fechaVisita) {
      this.error = 'Debe seleccionar una fecha de visita';
      return;
    }

    if (this.tipoEntrada === TipoTicket.EventoEspecial && !this.eventoSeleccionado) {
      this.error = 'Debe seleccionar un evento';
      return;
    }

    // Convertir la fecha a formato ISO DateTime con hora
    const fechaConHora = `${this.fechaVisita}T18:00:00`;

    const dto: CrearTicketDto = {
      fechaVisita: fechaConHora,
      tipoEntrada: this.tipoEntrada,
      eventoId: this.eventoSeleccionado
    };

    this.loading = true;

    this.ticketService.crearTicket(dto).subscribe({
      next: (ticket) => {
        this.success = '¡Ticket comprado exitosamente!';
        this.ticketCreado = ticket;
        this.loading = false;
        this.limpiarFormulario();
      },
      error: (err) => {
        this.loading = false;
        this.error = err.error?.mensaje || 'Error al comprar el ticket. Verifique el aforo disponible.';
        console.error('Error al crear ticket:', err);
      }
    });
  }

  limpiarFormulario(): void {
    this.fechaVisita = '';
    this.tipoEntrada = TipoTicket.General;
    this.eventoSeleccionado = undefined;
  }

  generarQRUrl(codigo: string): string {
    return `https://api.qrserver.com/v1/create-qr-code/?size=250x250&data=${codigo}`;
  }

  cerrarResultado(): void {
    this.ticketCreado = undefined;
    this.success = '';
  }
}