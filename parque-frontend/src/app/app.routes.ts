import { Routes } from '@angular/router';
import { HomeComponent } from './features/home/home';
import { AtraccionListComponent } from './features/atracciones/atraccion-list/atraccion-list';

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
    path: 'login',
    loadComponent: () => import('./features/auth/login/login')
      .then(m => m.LoginComponent)
  },

];
