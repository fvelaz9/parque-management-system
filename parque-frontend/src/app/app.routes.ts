import { Routes } from '@angular/router';
import { HomeComponent } from './features/home/home';
import { NotFoundComponent } from './features/not-found/not-found';
import {MantenimientoList} from './features/mantenimientos/mantenimiento-list/mantenimiento-list';
import {MantenimientoForm} from './features/mantenimientos/mantenimiento-form/mantenimiento-form';
import {RecompensaEditar} from './features/recompensas/recompensa-editar/recompensa-editar';
import {HistorialCanje} from './features/recompensas/recompensa-historial/recompensa-historial';
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
    path: 'mantenimientos',
    loadComponent: () => import('./features/mantenimientos/mantenimiento-list/mantenimiento-list')
      .then(m => m.MantenimientoList)
  },
  {
    path: 'mantenimientos/nuevo',
    loadComponent: () => import('./features/mantenimientos/mantenimiento-form/mantenimiento-form')
      .then(m => m.MantenimientoForm)
  },
  {
    path: 'recompensas',
    loadComponent: () => import('./features/recompensas/recompensas-list/recompensas-list')
      .then(m => m.RecompensasList)
  },
  {
    path: 'recompensas/nueva',
    loadComponent: () => import('./features/recompensas/recompensa-form/recompensa-form')
      .then(m => m.RecompensaForm)
  },
  {
    path: 'recompensas/editar/:id',
    loadComponent: () => import('./features/recompensas/recompensa-editar/recompensa-editar')
      .then(m => m.RecompensaEditar)
  },
  {
    path: 'recompensas/historial',
    loadComponent: () => import('./features/recompensas/recompensa-historial/recompensa-historial')
      .then(m => m.HistorialCanje)
  },
  {
    path: 'mantenimientos/editar/:id',
    loadComponent: () => import('./features/mantenimientos/mantenimiento-edit/mantenimiento-edit')
      .then(m => m.MantenimientoEdit)
  },
  {
    path: 'incidencias',
    loadComponent: () => import('./features/incidencias/incidencias-list/incidencias-list')
      .then(m => m.IncidenciasListComponent)
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
    canActivate: [operadorGuard]
  },
  {
    path: 'acceso-ingreso',
    loadComponent: () => import('./features/Acceso/acceso-ingreso/acceso-ingreso')
      .then(m => m.AccesoIngreso),
    canActivate: [operadorGuard]
  },
  {
    path: 'acceso-egreso',
    loadComponent: () => import('./features/Acceso/acceso-egreso/acceso-egreso')
      .then(m => m.AccesoEgreso),
    canActivate: [operadorGuard]
  },
  {
    path: 'configuracion/estrategias',
    loadComponent: () => import('./features/configuracion/selector-estrategias/selector-estrategias')
      .then(m => m.SelectorEstrategiasComponent),
    canActivate: [authGuard]
  },
  // Not Found route - debe ser la última
  {
    path: "**",
    component: NotFoundComponent
  },
];
