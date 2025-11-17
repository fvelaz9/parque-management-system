import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AtraccionesService } from '../../../core/services/atracciones.service';
import { AtraccionParque, AforoAtraccionDto } from '../../../core/models/atraccion.model';
import {CommonModule} from '@angular/common';

@Component({
  selector: 'app-acceso-home',
  templateUrl: './acceso-home.html',
  styleUrls: ['./acceso-home.css'],
  standalone: true,
  imports: [CommonModule],
})
export class AccesoHome implements OnInit {
  atraccionId!: number;
  atraccion: AtraccionParque | null = null;
  aforo: AforoAtraccionDto | null = null;

  constructor(
    private route: ActivatedRoute,
    private atraccionesService: AtraccionesService
  ) {}

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      this.atraccionId = +params['atraccionId'] || 0;

      if (this.atraccionId) {
        this.cargarDatos(this.atraccionId);
      }
    });
  }

  cargarDatos(id: number) {
    this.atraccionesService.getAtraccionById(id).subscribe({
      next: (res) => {
        this.atraccion = res;
      },
      error: (err) => {
        console.error('Error al obtener atracción', err);
      }
    });

    this.atraccionesService.getAforoActual(id).subscribe({
      next: (res: AforoAtraccionDto) => {
        this.aforo = res;
      },
      error: (err) => {
        console.error('Error al obtener aforo', err);
      }
    });
  }
}
