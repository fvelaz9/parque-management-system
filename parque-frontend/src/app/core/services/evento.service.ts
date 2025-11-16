import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CreateEventoRequest, Evento, EventoOutDto } from '../models/evento.model';
import { environment } from '../../../environments/environment.development';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class EventoService {
  private readonly apiUrl = `${environment.apiUrl}/eventos`;
  private readonly http = inject(HttpClient);
  private readonly authService = inject(AuthService);

  private getAuthHeaders(): HttpHeaders | undefined {
    const token = this.authService.getToken();
    return token ? new HttpHeaders({ 'Authorization': token }) : undefined;
  }

  listarEventos(): Observable<Evento[]> {
    const headers = this.getAuthHeaders();
    return this.http.get<Evento[]>(this.apiUrl, { headers });
  }

  crearEvento(evento: CreateEventoRequest): Observable<EventoOutDto> {
    const headers = this.getAuthHeaders();
    return this.http.post<EventoOutDto>(this.apiUrl, evento, { headers });
  }

  deleteEvento(id: number): Observable<void> {
    const headers = this.getAuthHeaders();
    return this.http.delete<void>(`${this.apiUrl}/${id}`, { headers });
  }
}
