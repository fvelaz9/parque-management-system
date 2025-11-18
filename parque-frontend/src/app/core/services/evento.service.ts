import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, pipe } from 'rxjs';
import { CreateEventoRequest, Evento, EventoOutDto, EstadoEvento } from '../models/evento.model';
import { environment } from '../../../environments/environment.development';

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
}
