import { Component, inject, signal, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AtraccionParque } from '../../../core/models/atraccion.model';
import { AtraccionesService } from '../../../core/services/atracciones.service';

@Component({
  selector: 'app-atraccion-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './atraccion-list.component.html',
  styleUrl: './atraccion-list.component.css'
})
export class AtraccionListComponent {
  private readonly atraccionesService = inject(AtraccionesService);
  
  public atracciones = signal<AtraccionParque[]>([]);
  public loading = signal<boolean>(true);
  public error = signal<string>('');

  // Effect para cargar las atracciones cuando se crea el componente
  private readonly loadAtraccionesEffect = effect(() => {
    this.atraccionesService.getAllAtracciones().subscribe({
      next: (result) => {
        console.log('Atracciones cargadas:', result);
        this.atracciones.set(result);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error al cargar atracciones:', err);
        this.error.set('No se pudieron cargar las atracciones. Verifica que el backend esté corriendo.');
        this.loading.set(false);
      }
    });
  });
}
