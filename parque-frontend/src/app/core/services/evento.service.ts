import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {CreateEventoRequest, Evento, EventoOutDto} from '../models/evento.model';
import { environment } from '../../../environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class EventoService {
  private readonly apiUrl = `${environment.apiUrl}/eventos`;
  private readonly http = inject(HttpClient);

  // Listar todos los eventos
  listarEventos(): Observable<Evento[]> {
    return this.http.get<Evento[]>(this.apiUrl);
  }

 // Crear un evento
  crearEvento(evento: CreateEventoRequest): Observable<EventoOutDto> {
    return this.http.post<EventoOutDto>(this.apiUrl, evento);
  }

  // Eliminar un evento
  deleteEvento(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
