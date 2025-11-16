import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import { ValidarAccesoRequest, ValidarAccesoResponse } from '../models/acceso.model';

@Injectable({
  providedIn: 'root',
})
export class AccesoService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/acceso`;

  validarAcceso(request: ValidarAccesoRequest): Observable<ValidarAccesoResponse> {
    return this.http.post<ValidarAccesoResponse>(`${this.apiUrl}/validar`, request);
  }

}
