import { Routes } from '@angular/router';
import { HomeComponent } from './features/home/home';
import { NotFoundComponent } from './features/not-found/not-found';
import { AtraccionListComponent } from './features/atracciones/atraccion-list/atraccion-list';
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
      .then(m => m.AtraccionListComponent),
      canActivate: [authGuard]
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
