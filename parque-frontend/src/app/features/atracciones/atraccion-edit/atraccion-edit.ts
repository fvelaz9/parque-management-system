import {Component, inject, OnInit} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {ActivatedRoute, Router, RouterLink} from '@angular/router';
import {NgForOf} from '@angular/common';

import { AtraccionParque, TipoAtraccion } from '../../../core/models/atraccion.model';
import {AtraccionesService} from '../../../core/services/atracciones.service';

@Component({
  selector: 'app-atraccion-edit',
  imports: [FormsModule, RouterLink, NgForOf],
  templateUrl: './atraccion-edit.html',
  styleUrl: './atraccion-edit.css',
  standalone: true
})
export class AtraccionEdit implements OnInit {
  atraccion: AtraccionParque = {
    id: 0,
    nombre: '',
    tipo: TipoAtraccion.MontañaRusa,
    edadMinima: 0,
    capacidad: 1,
    descripcion: '',
    estado: 0
  };

  tiposDisponibles = [
    { id: TipoAtraccion.MontañaRusa, nombre: 'Montaña Rusa' },
    { id: TipoAtraccion.Simulador, nombre: 'Simulador' },
    { id: TipoAtraccion.Espectaculo, nombre: 'Espectaculo' },
    { id: TipoAtraccion.ZonaInteractiva, nombre: 'Zona Interactiva' }
  ];

  private atraccionesService = inject(AtraccionesService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id) {
      this.atraccionesService.getAtraccionById(id).subscribe({
        next: (data) => {
          this.atraccion = data;
        },
        error: () => {
          alert('No se pudo cargar la atracción');
          this.router.navigate(['/atracciones']);
        }
      });
    }
  }

  onSubmit() {
    this.atraccionesService.updateAtraccion(this.atraccion.id, this.atraccion).subscribe({
      next: () => {
        alert('Atracción actualizada con éxito');
        this.router.navigate(['/atracciones']);
      },
      error: () => {
        alert('Error al actualizar la atracción');
      }
    });
  }
}
