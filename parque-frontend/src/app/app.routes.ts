import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    redirectTo: '/atracciones',
    pathMatch: 'full'
  },
  {
    path: 'atracciones',
    loadComponent: () => import('./features/atracciones/atraccion-list/atraccion-list')
      .then(m => m.AtraccionListComponent)
  }
];
