import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';
import { environment } from '../../../environments/environment.development';

export interface EstrategiaInfo {
  nombre: string;
  descripcion: string;
  origen: 'Base' | 'Plugin';
  esActiva: boolean;
  parametros?: string[];
}

export interface ResponseDto<T = any> {
  content: T;
  executionSuccessful: boolean;
  message: string;
}

export interface CambiarEstrategiaRequest {
  nombreEstrategia: string;
}

@Injectable({
  providedIn: 'root'
})
export class EstrategiasService {
  private http = inject(HttpClient);
  private authService = inject(AuthService);
  private apiUrl = `${environment.apiUrl}/cuentas`;

  private getHeaders(): HttpHeaders {
    const token = this.authService.getToken();
    return new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `${token}`
    });
  }

  obtenerEstrategiasDisponibles(): Observable<ResponseDto<EstrategiaInfo[]>> {
    return this.http.get<ResponseDto<EstrategiaInfo[]>>(`${this.apiUrl}/disponibles`, { headers: this.getHeaders() });
  }

  obtenerEstrategiaActiva(): Observable<ResponseDto<{ estrategiaActiva: string }>> {
    return this.http.get<ResponseDto<{ estrategiaActiva: string }>>(`${this.apiUrl}/activa`, { headers: this.getHeaders() });
  }

  cambiarEstrategiaActiva(nombreEstrategia: string): Observable<ResponseDto<{ nuevaEstrategia: string }>> {
    const request: CambiarEstrategiaRequest = { nombreEstrategia };
    return this.http.put<ResponseDto<{ nuevaEstrategia: string }>>(
      `${this.apiUrl}/activa`,
      request,
      { headers: this.getHeaders() }
    );
  }

  recargarPlugins(): Observable<ResponseDto<EstrategiaInfo[]>> {
    return this.http.post<ResponseDto<EstrategiaInfo[]>>(
      `${this.apiUrl}/plugins/recargar`,
      {},
      { headers: this.getHeaders() }
    );
  }
}