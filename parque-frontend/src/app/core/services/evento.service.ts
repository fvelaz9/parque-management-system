import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, pipe } from 'rxjs';
import { map } from 'rxjs/operators';
import { CreateEventoRequest, Evento, EventoOutDto, EstadoEvento } from '../models/evento.model';
import { environment } from '../../../environments/environment.development';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class EventoService {
  private readonly apiUrl = `${environment.apiUrl}/eventos`;
  private authService = inject(AuthService);
  private readonly http = inject(HttpClient);

  private getHeaders(): HttpHeaders {
    const token = this.authService.getToken();
    return new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `${token}`
    });
  }

  // Listar todos los eventos
  listarEventos(): Observable<Evento[]> {
    return this.http.get<Evento[]>(this.apiUrl, { headers: this.getHeaders() });
  }

  // Obtener eventos disponibles para compra de tickets
  obtenerEventosDisponibles(): Observable<Evento[]> {
    return this.listarEventos().pipe(
      map(eventos => eventos.filter(e => 
        e.estado === EstadoEvento.Programado || e.estado === EstadoEvento.Activo
      ))
    );
  }

  // Crear un evento
  crearEvento(evento: CreateEventoRequest): Observable<EventoOutDto> {
    return this.http.post<EventoOutDto>(this.apiUrl, evento, { headers: this.getHeaders() });
  }

  // Eliminar un evento
  deleteEvento(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`, { headers: this.getHeaders() });
  }
}
