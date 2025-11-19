// src/app/features/mis-tickets/mis-tickets.ts

import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { TicketService } from '../../core/services/ticket.service';
import { AuthService } from '../../core/services/auth.service';
import { Ticket, TipoTicket } from '../../core/models/ticket.model';

@Component({
  selector: 'app-mis-tickets',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './mis-tickets.html',
  styleUrl: './mis-tickets.css'
})
export class MisTicketsComponent implements OnInit {
  private readonly ticketService = inject(TicketService);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  public tickets = signal<Ticket[]>([]);
  public loading = signal(false);
  public error = signal('');

  ngOnInit(): void {
    this.cargarTickets();
  }

  private cargarTickets(): void {
    this.loading.set(true);
    this.error.set('');

    this.ticketService.obtenerMisTickets().subscribe({
      next: (tickets) => {
        this.tickets.set(tickets);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error al cargar tickets:', err);

        if (err.status === 401) {
          this.error.set('No estás autenticado');
          this.router.navigate(['/login']);
        } else if (err.status === 403) {
          this.error.set('No tienes permisos para ver los tickets');
        } else {
          this.error.set(err.error?.mensaje || 'Error al cargar los tickets');
        }

        this.loading.set(false);
      }
    });
  }

  obtenerNombreTipoTicket(tipo: TipoTicket): string {
    return tipo === TipoTicket.General ? 'Entrada General' : 'Evento Especial';
  }

  generarQRUrl(codigo: string): string {
    return `https://api.qrserver.com/v1/create-qr-code/?size=200x200&data=${codigo}`;
  }

  esTicketProximo(fechaVisita: string): boolean {
    const fecha = new Date(fechaVisita);
    const hoy = new Date();
    const diffDias = Math.ceil((fecha.getTime() - hoy.getTime()) / (1000 * 3600 * 24));
    return diffDias >= 0 && diffDias <= 7;
  }
}
