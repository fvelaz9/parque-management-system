import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { CuentaDto, LoginRequest, LoginResponse } from '../models/auth.model';
import { environment } from '../../../environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/sesiones`;

  private readonly TOKEN_KEY = 'token';
  private readonly USUARIO_KEY = 'usuario';

  login(credenciales: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, credenciales)
      .pipe(
        tap(response => this.guardarSesion(response))
      );
  }

  // Actualmente guardamos sesion en localStorage, es correcto?
  private guardarSesion(authResponse: LoginResponse): void {
    localStorage.setItem(this.TOKEN_KEY, authResponse.content.token);
    localStorage.setItem(this.USUARIO_KEY, JSON.stringify(authResponse.content.cuenta));
  }

  logout(): void {
    this.eliminarSesion();
  }

  private eliminarSesion(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.USUARIO_KEY);
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  getUsuario(): CuentaDto | null {
    const usuario = localStorage.getItem(this.USUARIO_KEY);
    return usuario ? JSON.parse(usuario) : null;
  }

  tieneRol(rol: string): boolean {
    const usuario = this.getUsuario();
    return usuario?.roles.includes(rol) ?? false;
  }
}