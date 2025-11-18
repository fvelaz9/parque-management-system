import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AtraccionParque, AforoAtraccionDto, ReporteAtraccionDto } from '../models/atraccion.model';
import { environment } from '../../../environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class AtraccionesService {
  private readonly apiUrl = `${environment.apiUrl}/atracciones`;
  private readonly http = inject(HttpClient);

  // GET /api/atracciones - Listar todas las atracciones
  getAllAtracciones(): Observable<AtraccionParque[]> {
    return this.http.get<AtraccionParque[]>(this.apiUrl);
  }

  // GET /api/atracciones/{id} - Obtener una atracción por ID
  getAtraccionById(id: number): Observable<AtraccionParque> {
    return this.http.get<AtraccionParque>(`${this.apiUrl}/${id}`);
  }

  // POST /api/atracciones - Crear una atracción
  createAtraccion(atraccion: AtraccionParque): Observable<AtraccionParque> {
    return this.http.post<AtraccionParque>(this.apiUrl, atraccion);
  }

  // PUT /api/atracciones/{id} - Actualizar una atracción
  updateAtraccion(id: number, atraccion: AtraccionParque): Observable<AtraccionParque> {
    return this.http.put<AtraccionParque>(`${this.apiUrl}/${id}`, atraccion);
  }

  // DELETE /api/atracciones/{id} - Eliminar una atracción
  deleteAtraccion(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  // GET /api/atracciones/{id}/aforo - Obtener aforo actual
  getAforoActual(id: number): Observable<AforoAtraccionDto> {
    return this.http.get<AforoAtraccionDto>(`${this.apiUrl}/${id}/aforo`);
  }

  // GET /api/atracciones/reporte?fechaInicio=...&fechaFin=... - Reporte de uso
  getReporteUso(fechaInicio: string, fechaFin: string): Observable<ReporteAtraccionDto[]> {
    return this.http.get<ReporteAtraccionDto[]>(`${this.apiUrl}/reporte`, {
      params: { fechaInicio, fechaFin }
    });
  }
}
