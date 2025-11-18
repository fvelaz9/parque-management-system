// src/app/core/services/gamificacion.service.ts

import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { HistorialPuntuacionDto } from '../models/gamificacion.model';
import { environment } from '../../../environments/environment.development';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class GamificacionService {
  private readonly apiUrl = `${environment.apiUrl}/gamificacion`;
  private readonly http = inject(HttpClient);
  private readonly authService = inject(AuthService);

  private getHeaders(): HttpHeaders {
    const token = this.authService.getToken();
    return new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `${token}`
    });
  }

  // GET /api/gamificacion/ranking/diario - Obtener ranking diario
  getRankingDiario(fecha?: string, top: number = 10): Observable<any> {
    let params = new HttpParams().set('top', top.toString());

    if (fecha) {
      params = params.set('fecha', fecha);
    }

    return this.http.get<any>(`${this.apiUrl}/ranking/diario`, {
      headers: this.getHeaders(),
      params
    });
  }

  // GET /api/gamificacion/historial - Obtener historial de puntuaciones de un visitante
  getHistorialVisitante(visitanteId: string): Observable<any> {
    const params = new HttpParams().set('visitanteId', visitanteId);

    return this.http.get<any>(`${this.apiUrl}/historial`, {
      headers: this.getHeaders(),
      params
    });
  }
}
