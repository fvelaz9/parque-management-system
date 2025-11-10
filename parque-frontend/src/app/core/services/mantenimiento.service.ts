// src/app/core/services/mantenimientos.service.ts
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { MantenimientoPreventivo, CrearMantenimientoRequest } from '../models/mantenimiento.model';
import { environment } from '../../../environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class MantenimientosService {
  private readonly apiUrl = `${environment.apiUrl}/mantenimientos`;
  private readonly http = inject(HttpClient);

  // GET /api/mantenimientos - Listar todos los mantenimientos
  getAllMantenimientos(): Observable<MantenimientoPreventivo[]> {
    return this.http.get<MantenimientoPreventivo[]>(this.apiUrl);
  }

  // POST /api/mantenimientos - Crear un mantenimiento
  createMantenimiento(request: CrearMantenimientoRequest): Observable<MantenimientoPreventivo> {
    return this.http.post<MantenimientoPreventivo>(this.apiUrl, request);
  }

  // DELETE /api/mantenimientos/{id} - Eliminar un mantenimiento
  deleteMantenimiento(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
