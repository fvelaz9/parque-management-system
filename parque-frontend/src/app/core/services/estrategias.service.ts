import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import { ResponseDto, EstrategiaInfo, CambiarEstrategiaRequest } from '../models/estrategia.model';

@Injectable({
  providedIn: 'root'
})
export class EstrategiasService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/estrategias`;

  obtenerEstrategiasDisponibles(): Observable<ResponseDto<EstrategiaInfo[]>> {
    return this.http.get<ResponseDto<EstrategiaInfo[]>>(`${this.apiUrl}/disponibles`);
  }

  obtenerEstrategiaActiva(): Observable<ResponseDto<{ estrategiaActiva: string }>> {
    return this.http.get<ResponseDto<{ estrategiaActiva: string }>>(`${this.apiUrl}/activa`);
  }

  cambiarEstrategiaActiva(nombreEstrategia: string): Observable<ResponseDto<{ nuevaEstrategia: string }>> {
    const request: CambiarEstrategiaRequest = { nombreEstrategia };
    return this.http.put<ResponseDto<{ nuevaEstrategia: string }>>(
      `${this.apiUrl}/activa`, request);
  }

  recargarPlugins(): Observable<ResponseDto<EstrategiaInfo[]>> {
    return this.http.post<ResponseDto<EstrategiaInfo[]>>(
      `${this.apiUrl}/plugins/recargar`,
      {});
  }
}