// src/app/core/services/recompensas.service.ts
import { Injectable, inject } from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import { Observable } from 'rxjs';
import { Recompensa, CrearRecompensaRequest, CanjearRecompensaRequest, HistorialCanjeDto } from '../models/recompensa.model';
import { environment } from '../../../environments/environment.development';
import { AuthService } from './auth.service';


@Injectable({ providedIn: 'root' })
export class RecompensasService {
  private readonly http = inject(HttpClient);
  private authService = inject(AuthService);
  private readonly apiUrl = `${environment.apiUrl}/recompensas`;

  private getHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');
    return new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': token || ''  // ← SIN "Bearer"
    });
  }

  // GET /api/recompensas
  getAll(): Observable<{ total: number; recompensas: Recompensa[] }> {
    return this.http.get<{ total: number; recompensas: Recompensa[] }>(this.apiUrl);
  }

  // GET /api/recompensas/{id}
  getById(id: string): Observable<Recompensa> {
    return this.http.get<Recompensa>(`${this.apiUrl}/${id}`, {headers: this.getHeaders()});
  }

  // POST /api/recompensas
  create(request: CrearRecompensaRequest): Observable<any> {
    return this.http.post(this.apiUrl, request);
  }

  // PUT /api/recompensas/{id}
  update(id: string, request: CrearRecompensaRequest): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, request);
  }

  // POST /api/recompensas/canjear
  canjear(request: CanjearRecompensaRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/canjear`, request);
  }

  // GET /api/recompensas/historial/{visitanteId}
  getHistorial(visitanteId: string): Observable<{ visitanteId: string; totalCanjes: number; historial: HistorialCanjeDto[] }> {
    return this.http.get<{ visitanteId: string; totalCanjes: number; historial: HistorialCanjeDto[] }>(`${this.apiUrl}/historial/${visitanteId}`);
  }
}
