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
import {ReportesComponent} from './features/reportes/reportes';
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
      .then(m => m.AtraccionListComponent),
    canActivate: [authGuard]
  },
  {
    path: 'atracciones/nueva',
    loadComponent: () => import('./features/atracciones/atraccion-form/atraccion-form')
      .then(m => m.AtraccionForm),
    canActivate: [adminGuard]
  },
  {
    path: 'atracciones/editar/:id',
    loadComponent: () => import('./features/atracciones/atraccion-edit/atraccion-edit')
      .then(m => m.AtraccionEdit),
    canActivate: [adminGuard]
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
      .then(m => m.MantenimientoList),
    canActivate: [adminGuard, operadorGuard]
  },
  {
    path: 'mantenimientos/nuevo',
    loadComponent: () => import('./features/mantenimientos/mantenimiento-form/mantenimiento-form')
      .then(m => m.MantenimientoForm),
    canActivate: [adminGuard]
  },
  {
    path: 'recompensas',
    loadComponent: () => import('./features/recompensas/recompensas-list/recompensas-list')
      .then(m => m.RecompensasList),
    canActivate: [adminGuard, visitanteGuard]
  },
  {
    path: 'recompensas/nueva',
    loadComponent: () => import('./features/recompensas/recompensa-form/recompensa-form')
      .then(m => m.RecompensaForm),
    canActivate: [adminGuard]
  },
  {
    path: 'recompensas/editar/:id',
    loadComponent: () => import('./features/recompensas/recompensa-editar/recompensa-editar')
      .then(m => m.RecompensaEditar),
    canActivate: [adminGuard]
  },
  {
    path: 'recompensas/historial',
    loadComponent: () => import('./features/recompensas/recompensa-historial/recompensa-historial')
      .then(m => m.HistorialCanje),
    canActivate: [visitanteGuard]
  },
  {
    path: 'mantenimientos/editar/:id',
    loadComponent: () => import('./features/mantenimientos/mantenimiento-edit/mantenimiento-edit')
      .then(m => m.MantenimientoEdit),
    canActivate: [adminGuard]
  },
  {
    path: 'incidencias',
    loadComponent: () => import('./features/incidencias/incidencias-list/incidencias-list')
      .then(m => m.IncidenciasListComponent),
    canActivate: [operadorGuard]
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
    canActivate: [adminGuard]
  },
  {
    path: 'evento-atracciones',
    loadComponent: () => import('./features/Evento/evento-atracciones/evento-atracciones')
      .then(m => m.EventoAtracciones),
    canActivate: [operadorGuard]
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
    canActivate: [adminGuard]
  },
  {
    path: 'ranking',
    loadComponent: () => import('./features/ranking/ranking')
      .then(m => m.RankingComponent),
    canActivate: [adminGuard]
  },
  {
    path: 'reportes',
    loadComponent: () => import('./features/reportes/reportes')
      .then(m => m.ReportesComponent),
    canActivate: [adminGuard]
  },
  {
    path: 'tickets/comprar',
    loadComponent: () => import('./features/ticket/ticket')
      .then(m => m.ComprarTicketComponent),
    canActivate: [visitanteGuard]
  },
  {
    path: 'historial-puntuacion',
    loadComponent: () => import('./features/historial-puntuacion/historial-puntuacion')
      .then(m => m.HistorialPuntuacionComponent),
    path: 'acceso-denegado',
    loadComponent: () => import('./features/acceso-denegado/acceso-denegado')
      .then(m => m.AccesoDenegado)
  },
  // Not Found route - debe ser la última
  {
    path: "**",
    component: NotFoundComponent
  },
];
