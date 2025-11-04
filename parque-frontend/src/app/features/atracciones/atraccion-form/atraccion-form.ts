import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { NgForOf } from '@angular/common';

import { AtraccionParque, TipoAtraccion } from '../../../core/models/atraccion.model';
import {AtraccionesService} from '../../../core/services/atracciones.service';

@Component({
  selector: 'app-atraccion-form',
  templateUrl: './atraccion-form.html',
  styleUrls: ['./atraccion-form.css'],
  imports: [FormsModule, RouterLink, NgForOf],
  standalone: true
})
export class AtraccionForm {
  atraccion: AtraccionParque = {
    id: 0, // en creación puede ser 0 o omitido según backend
    nombre: '',
    tipo: TipoAtraccion.MontañaRusa, // valor inicial por defecto
    edadMinima: 0,
    capacidad: 1,
    descripcion: '',
    estado: 0 // o asigna un estado por defecto válido
  };

  tiposDisponibles = [
    { id: TipoAtraccion.MontañaRusa, nombre: 'Montaña Rusa' },
    { id: TipoAtraccion.Simulador, nombre: 'Simulador' },
    { id: TipoAtraccion.Espectaculo, nombre: 'Espectaculo' },
    { id: TipoAtraccion.ZonaInteractiva, nombre: 'Zona Interactiva' }
  ];
  constructor(private atraccionesService: AtraccionesService) {}
  onSubmit() {
    console.log('Enviando atracción:', this.atraccion);

    this.atraccionesService.createAtraccion(this.atraccion).subscribe({
      next: (res) => {
        alert('Atracción creada con éxito');
      },
      error: (err) => {
        console.error('Error creando atracción:', err);
        alert('Error al crear atracción');
      }
    });
  }
}
