// src/app/features/recompensas/recompensas-list/recompensas-list.ts
import { Component, effect, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { RecompensasService } from '../../../core/services/recompensa.service';
import { AuthService } from '../../../core/services/auth.service';
import { Recompensa, NivelMembresia } from '../../../core/models/recompensa.model';

@Component({
  selector: 'app-recompensas-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './recompensas-list.html',
  styleUrl: './recompensas-list.css'
})
export class RecompensasList {
  private readonly recompensasService = inject(RecompensasService);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  total = signal(0);
  puntosVisitante = signal(0);
  recompensas = signal<Recompensa[]>([]);
  loading = signal(true);
  error = signal('');

  private readonly loadEffect = effect(() => {
    const usuario = this.authService.getUsuario();
    this.cargarRecompensas();
    this.cargarPuntosVisitante();
  });

  private cargarRecompensas() {
    this.loading.set(true);
    this.error.set('');
    this.recompensasService.getAll().subscribe({
      next: (resp) => {
        this.total.set(resp.total);
        this.recompensas.set(resp.recompensas);
        this.loading.set(false);
        this.cargarPuntosVisitante();
      },
      error: (err) => {
        console.error('Error al cargar recompensas:', err);
        this.error.set('Error al cargar las recompensas');
        this.loading.set(false);
      }
    });
  }
  cargarPuntosVisitante() {
    const visitanteId = this.visitanteId;
    if (!visitanteId) return;

    this.recompensasService.getPuntosVisitante(visitanteId).subscribe({
      next: (resp) => this.puntosVisitante.set(resp.puntos),
      error: () => this.puntosVisitante.set(0)
    });
  }

  get isAdmin(): boolean {
    return this.authService.getUsuario()?.roles?.includes('Administrador') || false;
  }

  get isVisitante(): boolean {
    return this.authService.getUsuario()?.roles?.includes('Visitante') || false;
  }

  get visitanteId(): string | null {
    const usuario = this.authService.getUsuario();
    return usuario?.visitante?.id || null;
  }

  canjearRecompensa(recompensa: Recompensa) {
    if (!this.visitanteId) {
      alert('No se pudo obtener el ID del visitante');
      return;
    }

    const confirmar = confirm(`¿Canjear "${recompensa.nombre}" por ${recompensa.costoEnPuntos} puntos?`);
    if (!confirmar) return;

    this.recompensasService.canjear({
      visitanteId: this.visitanteId,
      recompensaId: recompensa.id
    }).subscribe({
      next: (resp) => {
        alert(resp.message || 'Recompensa canjeada exitosamente');
        this.cargarRecompensas();
      },
      error: (err) => {
        console.error('Error al canjear:', err);
        alert(err.error?.message || 'Error al canjear la recompensa');
      }
    });
  }


  verHistorial() {
    this.router.navigate(['/recompensas/historial']);
  }

  readonly nivelesMap = {
    1: 'Estandar',
    2: 'Premium',
    3: 'VIP'
  } as const;

  obtenerNivel(nivel?: number): string {
    if (!nivel) return 'Sin requisito';
    return (this.nivelesMap as any)[nivel] || 'Sin requisito';
  }

  obtenerClaseNivel(nivel?: number): string {
    if (!nivel) return '';
    const nombreNivel = (this.nivelesMap as any)[nivel];
    if (!nombreNivel) return '';
    return `nivel-${nombreNivel.toLowerCase()}`;
  }
  editarRecompensa(id: string) {
    this.router.navigate(['/recompensas/editar', id]);
  }

  eliminarRecompensa(recompensa: Recompensa) {
    const confirmar = confirm(`¿Estás seguro de eliminar "${recompensa.nombre}"?`);
    if (!confirmar) return;

    this.recompensasService.delete(recompensa.id).subscribe({
      next: () => {
        alert('Recompensa eliminada exitosamente');
        this.cargarRecompensas();
      },
      error: (err) => {
        console.error('Error al eliminar:', err);
        alert(err.error?.mensaje || 'Error al eliminar la recompensa');
      }
    });
  }
  get nombreVisitante(): string {
    const usuario = this.authService.getUsuario();
    return usuario?.nombre || 'Visitante';
  }
}
