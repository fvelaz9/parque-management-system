import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Ticket, CrearTicketDto } from '../models/ticket.model';
import { environment } from '../../../environments/environment.development';


@Injectable({
  providedIn: 'root'
})
export class TicketService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/tickets`;

  crearTicket(dto: CrearTicketDto): Observable<Ticket> {
    return this.http.post<Ticket>(this.apiUrl, dto);
  }

  obtenerMisTickets(): Observable<Ticket[]> {
    return this.http.get<Ticket[]>(`${this.apiUrl}/mis-tickets`);
  }

  getTicketsPorUsuarioYEvento(usuarioId: string, eventoId: number): Observable<Ticket[]> {
    return this.http.get<Ticket[]>(
        `${this.apiUrl}/por-usuario/${usuarioId}/evento/${eventoId}`
    );
  }

  getTicketsPorUsuario(usuarioId: string): Observable<Ticket[]> {
    return this.http.get<Ticket[]>(
        `${this.apiUrl}/por-usuario/${usuarioId}`
    );
  }
}
