import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const visitanteGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  // Primero verificar si está autenticado
  if (!authService.isLoggedIn()) {
    router.navigate(['/login']);
    return false;
  }

  // Luego verificar si tiene rol de visitante
  if (!authService.tieneRol('Visitante')) {
    router.navigate(['/home']);
    return false;
  }

  return true;
};