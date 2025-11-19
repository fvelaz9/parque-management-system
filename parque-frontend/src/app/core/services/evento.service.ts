import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CreateEventoRequest, Evento, EventoOutDto } from '../models/evento.model';
import { environment } from '../../../environments/environment.development';
import {AtraccionParque} from '../models/atraccion.model';

@Injectable({
  providedIn: 'root'
})
export class EventoService {
  private readonly apiUrl = `${environment.apiUrl}/eventos`;
  private readonly http = inject(HttpClient);

  listarEventos(): Observable<Evento[]> {
    return this.http.get<Evento[]>(this.apiUrl);
  }

  crearEvento(evento: CreateEventoRequest): Observable<EventoOutDto> {
    return this.http.post<EventoOutDto>(this.apiUrl, evento);
  }

  deleteEvento(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
  obtenerAtraccionesPorEvento(eventoId: number): Observable<AtraccionParque[]> {
    return this.http.get<AtraccionParque[]>(
      `${this.apiUrl}/${eventoId}/atracciones`
    );
  }
}
