import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import { AuthService } from './auth.service';
import { Ticket } from '../models/ticket.model';

@Injectable({
  providedIn: 'root',
})
export class TicketService {
  private http = inject(HttpClient);
  private authService = inject(AuthService);

  private apiUrl = `${environment.apiUrl}/tickets`;

  private getHeaders(): HttpHeaders {
    const token = this.authService.getToken();
    return new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': token ?? ''
    });
  }


  getTicketsPorUsuario(usuarioId: string): Observable<Ticket[]> {
    return this.http.get<Ticket[]>(`${this.apiUrl}/por-usuario/${usuarioId}`, {
      headers: this.getHeaders(),
    });
  }
  getTicketsPorUsuarioYEvento(usuarioId: string, eventoId: number): Observable<Ticket[]> {
    return this.http.get<Ticket[]>(
        `${this.apiUrl}/por-usuario/${usuarioId}/evento/${eventoId}`,
        { headers: this.getHeaders() }
    );
  }
}
