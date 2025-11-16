// src/app/core/services/mantenimientos.service.ts
import { Injectable, inject } from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import { Observable } from 'rxjs';
import { MantenimientoPreventivo, CrearMantenimientoRequest } from '../models/mantenimiento.model';
import { environment } from '../../../environments/environment.development';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class MantenimientosService {
  private readonly apiUrl = `${environment.apiUrl}/mantenimientos`;
  private readonly http = inject(HttpClient);
  private readonly authService = inject(AuthService);

  private getHeaders(): HttpHeaders {
    const token = this.authService.getToken();
    return new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `${token}`
    });
  }
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

  updateMantenimiento(id : number, request: CrearMantenimientoRequest): Observable<any>{
    return this.http.put(`${this.apiUrl}/${id}`, request, { headers: this.getHeaders() });
  }
}
