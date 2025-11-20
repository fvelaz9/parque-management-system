
import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Incidencia } from '../models/incidencia.model';
import { environment } from '../../../environments/environment.development';

@Injectable({ providedIn: 'root' })
export class IncidenciasService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/incidencias`;

  getAllIncidencias(): Observable<Incidencia[]> {
    return this.http.get<Incidencia[]>(this.apiUrl);
  }
  resolverIncidencia(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
