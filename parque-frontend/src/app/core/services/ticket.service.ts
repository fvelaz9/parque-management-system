import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Ticket, CrearTicketDto } from '../models/ticket.model';
import { environment } from '../../../environments/environment.development';
import { AuthService } from './auth.service';


@Injectable({
  providedIn: 'root'
})
export class TicketService {
  private http = inject(HttpClient);
  private authService = inject(AuthService);
  private apiUrl = `${environment.apiUrl}/tickets`;

  private getHeaders(): HttpHeaders {
    const token = this.authService.getToken();
    return new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `${token}`
    });
  }

  crearTicket(dto: CrearTicketDto): Observable<Ticket> {
    return this.http.post<Ticket>(this.apiUrl, dto, {
      headers: this.getHeaders()
    });
  }

  obtenerMisTickets(): Observable<Ticket[]> {
    return this.http.get<Ticket[]>(`${this.apiUrl}/mis-tickets`, {
      headers: this.getHeaders()
    });
  }
}