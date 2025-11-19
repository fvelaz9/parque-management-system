import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {FechaHoraService} from '../../core/services/fechaHora.service';
import {FechaActualResponse} from '../../core/models/fechaHora.model';

@Component({
  selector: 'app-fecha-hora',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './fecha-hora.html',
  styleUrls: ['./fecha-hora.css']
})
export class FechaHora implements OnInit {

  fechaHoraActual: string = '';
  fechaHoraConfigurar: string = '';
  mensaje: string = '';
  error: string = '';

  constructor(private fechaHoraService: FechaHoraService) { }

  ngOnInit(): void {
    this.cargarFechaHoraActual();
  }

  cargarFechaHoraActual(): void {
    this.fechaHoraService.obtenerFechaActual().subscribe({
      next: (res: FechaActualResponse) => {
        this.fechaHoraActual = res.datetime;
        this.fechaHoraConfigurar = res.datetime.substring(0, 16);
        this.mensaje = '';
        this.error = '';
      },
      error: () => {
        this.error = 'Error al obtener la fecha actual';
      }
    });
  }

  configurarFecha(): void {
    if (!this.fechaHoraConfigurar) {
      this.error = 'Debe ingresar una fecha y hora para configurar.';
      return;
    }

    this.fechaHoraService.configurarFecha(this.fechaHoraConfigurar).subscribe({
      next: () => {
        this.mensaje = 'Fecha configurada exitosamente';
        this.error = '';
        this.cargarFechaHoraActual();
      },
      error: (err) => {
        if (err?.error?.message) {
          this.error = err.error.message;
        } else {
          this.error = 'Error al configurar la fecha';
        }
        this.mensaje = '';
      }
    });
  }


  resetearFecha(): void {
    this.fechaHoraService.resetearFecha().subscribe({
      next: () => {
        this.mensaje = 'Fecha reseteada al sistema exitosamente';
        this.error = '';
        this.cargarFechaHoraActual();
      },
      error: () => {
        this.error = 'Error al resetear la fecha';
      }
    });
  }
}
