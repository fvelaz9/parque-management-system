import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { AuthService } from './auth.service'
import {FechaActualResponse} from '../models/fechaHora.model';
import {Observable} from 'rxjs';
import {environment} from '../../../environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class FechaHoraService {
  private http = inject(HttpClient);
  private authService = inject(AuthService);
  private readonly apiUrl = `${environment.apiUrl}/fecha-hora`;

  private getHeaders(): HttpHeaders {
    const token = this.authService.getToken();
    return new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': token ? `Bearer ${token}` : ''
    });
  }

  obtenerFechaActual(): Observable<FechaActualResponse> {
    return this.http.get<FechaActualResponse>(this.apiUrl, { headers: this.getHeaders() });
  }

  configurarFecha(fechaHora: string) {
    return this.http.put(this.apiUrl, { FechaHora: fechaHora }, { headers: this.getHeaders() });
  }

  resetearFecha() {
    return this.http.delete(this.apiUrl, { headers: this.getHeaders() });
  }
}
