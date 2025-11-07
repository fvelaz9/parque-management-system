import { Routes } from '@angular/router';
import { HomeComponent } from './features/home/home';
import { NotFoundComponent } from './features/not-found/not-found';
import { authGuard } from './auth-guard';

export const routes: Routes = [
  {
    path: '',
    redirectTo: '/home',  // Cambiar a home
    pathMatch: 'full'
  },
  {
    path: 'home',
    component: HomeComponent  // Ruta para home
  },
  {
    path: 'atracciones',
    loadComponent: () => import('./features/atracciones/atraccion-list/atraccion-list')
      .then(m => m.AtraccionListComponent)
  },
  {
    path: 'atracciones/nueva',
    loadComponent: () => import('./features/atracciones/atraccion-form/atraccion-form')
      .then(m => m.AtraccionForm)
  },
  {
    path: 'atracciones/editar/:id',
    loadComponent: () => import('./features/atracciones/atraccion-edit/atraccion-edit')
      .then(m => m.AtraccionEdit)
  },
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login/login')
      .then(m => m.LoginComponent)
  },
  {
    path: 'registro-visitante',
    loadComponent: () => import('./features/cuentas/registro-visitante/registro-visitante')
      .then(m => m.RegistroVisitanteComponent)
  },
  // Not Found route - debe ser la última
  {
    path: "**",
    component: NotFoundComponent
  }

];
