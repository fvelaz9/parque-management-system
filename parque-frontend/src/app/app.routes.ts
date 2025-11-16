import { Routes } from '@angular/router';
import { HomeComponent } from './features/home/home';
import { NotFoundComponent } from './features/not-found/not-found';
import { authGuard } from './core/guards/auth.guard';
import { visitanteGuard } from './core/guards/visitante.guard';
import { adminGuard } from './core/guards/admin.guard';
import {operadorGuard} from './core/guards/operador.guard';

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
  {
    path: 'eventos',
    loadComponent: () => import('./features/Evento/evento-list/evento-list.component')
      .then(m => m.EventoListComponent),
    canActivate: [authGuard]
  },
  {
    path: 'eventos/nuevo',
    loadComponent: () => import('./features/Evento/evento-form/evento-form')
      .then(m => m.EventoForm),
    canActivate: [authGuard]
  },
  {
   path: 'cuentas',
    loadComponent: () => import('./features/cuentas/lista-cuentas/lista-cuentas')
      .then(m => m.ListaCuentasComponent),
    canActivate: [adminGuard]
  },
  {
    path: 'cuentas/crear',
    loadComponent: () => import('./features/cuentas/crear-cuenta/crear-cuenta')
      .then(m => m.CrearCuentaComponent),
    canActivate: [adminGuard]
  },
  {
    path: 'modificar-perfil',
    loadComponent: () => import('./features/cuentas/modificar-perfil/modificar-perfil')
      .then(m => m.ModificarPerfilComponent),
    canActivate: [visitanteGuard]
  },
  {
    path: 'acceso',
    loadComponent: () => import('./features/Acceso/acceso-home/acceso-home')
  .then(m => m.AccesoHome),
  canActivate: [authGuard]
  },
  {
    path: 'acceso/validar',
    loadComponent: () => import('./features/Acceso/acceso-validar/acceso-validar')
      .then(m => m.AccesoValidar)
    // canActivate: [operadorGuard]
  },
  // Not Found route - debe ser la última
  {
    path: "**",
    component: NotFoundComponent
  },



];
