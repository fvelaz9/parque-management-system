import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  RegistrarVisitanteDto,
  RegistrarCuentaDto,
  ModificarPerfilDto,
  NivelMembresia,
  ResponseDto,
  CuentaDto
} from '../models/cuenta.model';
import { environment } from '../../../environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class CuentaService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/cuentas`;

  obtenerCuentas(): Observable<ResponseDto<CuentaDto[]>> {
    return this.http.get<ResponseDto<CuentaDto[]>>(this.apiUrl);
  }

  registrarVisitante(dto: RegistrarVisitanteDto): Observable<ResponseDto<CuentaDto>> {
    return this.http.post<ResponseDto<CuentaDto>>(`${this.apiUrl}/registro`, dto);
  }

  crearCuenta(dto: RegistrarCuentaDto): Observable<ResponseDto<CuentaDto>> {
    return this.http.post<ResponseDto<CuentaDto>>(this.apiUrl, dto);
  }

  modificarPerfil(dto: ModificarPerfilDto): Observable<ResponseDto<null>> {
    return this.http.put<ResponseDto<null>>(`${this.apiUrl}/perfil`, dto);
  }

  cambiarNivelMembresia(id: string, nuevoNivel: NivelMembresia): Observable<ResponseDto<null>> {
    return this.http.patch<ResponseDto<null>>(
      `${this.apiUrl}/${id}/membresia`,
      nuevoNivel,
    );
  }
  obtenerCuentasVisitantes(): Observable<ResponseDto<CuentaDto[]>> {
    return this.http.get<ResponseDto<CuentaDto[]>>(`${this.apiUrl}/visitantes`, {
      headers: this.getHeaders()
    });
  }
}
