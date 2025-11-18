import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';
import {
  AforoResponse,
  RegistrarEgresoRequest, RegistrarEgresoResponse,
  RegistrarIngresoRequest,
  RegistroVisitaDto
} from '../models/acceso.model';
import { environment } from '../../../environments/environment.development';
import {ResponseDto} from '../models/cuenta.model';

@Injectable({
  providedIn: 'root',
})
export class AccesoService {
  private http = inject(HttpClient);
  private authService = inject(AuthService);

  private apiUrl = `${environment.apiUrl}/acceso`;

  private getHeaders(): HttpHeaders {
    const token = this.authService.getToken();
    return new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': token ?? ''
    });
  }


  obtenerAforo(atraccionId: number): Observable<AforoResponse> {
    return this.http.get<AforoResponse>(`${this.apiUrl}/atraccion/${atraccionId}/aforo`, {
      headers: this.getHeaders(),
    });
  }
  registrarIngreso(atraccionId: number, request: RegistrarIngresoRequest): Observable<any> {
    return this.http.post(
      `${this.apiUrl}/atraccion/${atraccionId}/ingreso`,
      request,
      { headers: this.getHeaders() }
    );
  }
  obtenerRegistrosActivos(usuarioId: string, atraccionId: number): Observable<RegistroVisitaDto[]> {
    return this.http.get<RegistroVisitaDto[]>(
      `${this.apiUrl}/registros-activos/${usuarioId}/${atraccionId}`,  // ← Agregar atraccionId
      { headers: this.getHeaders() }
    );
  }
  registrarEgreso(atraccionId: number, request: RegistrarEgresoRequest): Observable<ResponseDto<RegistrarEgresoResponse>> {
    return this.http.post<ResponseDto<RegistrarEgresoResponse>>(
      `${this.apiUrl}/atraccion/${atraccionId}/egreso`, request, { headers: this.getHeaders() }
    );
  }

}
