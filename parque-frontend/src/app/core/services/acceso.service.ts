import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';
import { AforoResponse } from '../models/acceso.model';
import { environment } from '../../../environments/environment.development';

@Injectable({
  providedIn: 'root',
})
export class AccesoService {
  private http = inject(HttpClient);
  private authService = inject(AuthService);

  private apiUrl = `${environment.apiUrl}/acceso`;

  private getAuthHeaders(): HttpHeaders | undefined {
    const token = this.authService.getToken();
    return token ? new HttpHeaders({ Authorization: `Bearer ${token}` }) : undefined;
  }

  obtenerAforo(atraccionId: number): Observable<AforoResponse> {
    return this.http.get<AforoResponse>(`${this.apiUrl}/atraccion/${atraccionId}/aforo`, {
      headers: this.getAuthHeaders(),
    });
  }
}
