// src/app/features/incidencias/incidencias-list/incidencias-list.component.ts
import { Component, inject, signal, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Incidencia } from '../../../core/models/incidencia.model';
import { IncidenciasService } from '../../../core/services/incidencias.service';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-incidencias-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './incidencias-list.html',
  styleUrl: './incidencias-list.css'
})
export class IncidenciasListComponent {
  private readonly incidenciasService = inject(IncidenciasService);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  public incidencias = signal<Incidencia[]>([]);
  public loading = signal(true);
  public error = signal('');

  private readonly loadIncidenciasEffect = effect(() => {
    this.cargarIncidencias();
  });

  private cargarIncidencias() {
    this.loading.set(true);
    this.incidenciasService.getAllIncidencias().subscribe({
      next: (result) => {
        console.log('Incidencias cargadas:', result);
        this.incidencias.set(result);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error al cargar incidencias:', err);
        this.error.set('Error al cargar las incidencias');
        this.loading.set(false);
      }
    });
  }

  get isAdmin(): boolean {
    return this.authService.getUsuario()?.roles?.includes('Administrador') || false;
  }

  get isOperador(): boolean {
    return this.authService.getUsuario()?.roles?.includes('Operador') || false;
  }

  get puedeGestionar(): boolean {
    return this.isAdmin || this.isOperador;
  }
}
