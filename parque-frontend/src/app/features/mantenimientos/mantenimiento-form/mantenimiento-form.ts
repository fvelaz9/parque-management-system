import { Component, inject, signal, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { CrearMantenimientoRequest } from '../../../core/models/mantenimiento.model';
import { MantenimientosService } from '../../../core/services/mantenimiento.service';
import { AtraccionesService } from '../../../core/services/atracciones.service';
import { AtraccionParque } from '../../../core/models/atraccion.model';

@Component({
  selector: 'app-mantenimiento-form',
  templateUrl: './mantenimiento-form.html',
  styleUrls: ['./mantenimiento-form.css'],
  imports: [FormsModule, RouterLink, CommonModule],
  standalone: true
})
export class MantenimientoForm implements OnInit {
  private readonly router = inject(Router);
  private readonly mantenimientosService = inject(MantenimientosService);
  private readonly atraccionesService = inject(AtraccionesService);

  public atracciones = signal<AtraccionParque[]>([]);
  public loading = signal(true);
  public submitting = signal(false);

  mantenimiento: CrearMantenimientoRequest = {
    atraccionId: 0,
    fechaProgramada: '',
    horaInicio: '',
    duracionEstimada: '',
    descripcion: ''
  };

  ngOnInit() {
    this.cargarAtracciones();
  }

  private cargarAtracciones() {
    this.loading.set(true);
    this.atraccionesService.getAllAtracciones().subscribe({
      next: (result) => {
        console.log('Atracciones cargadas:', result);
        this.atracciones.set(result);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error al cargar atracciones:', err);
        alert('Error al cargar las atracciones');
        this.loading.set(false);
      }
    });
  }

  onSubmit() {
    if (!this.validarFormulario()) {
      return;
    }

    this.submitting.set(true);

    // Convertir duración de horas a formato HH:mm:ss
    const duracionFormatoAPI = this.convertirDuracionATimeSpan(this.mantenimiento.duracionEstimada);

    const request: CrearMantenimientoRequest = {
      ...this.mantenimiento,
      duracionEstimada: duracionFormatoAPI,
      horaInicio: this.mantenimiento.horaInicio + ':00' // Agregar segundos
    };

    console.log('Enviando mantenimiento:', request);

    this.mantenimientosService.createMantenimiento(request).subscribe({
      next: (res) => {
        console.log('Mantenimiento creado:', res);
        alert('Mantenimiento programado con éxito');
        this.router.navigate(['/mantenimientos']);
      },
      error: (err) => {
        console.error('Error al crear mantenimiento:', err);
        alert(err.error?.mensaje || 'Error al crear el mantenimiento');
        this.submitting.set(false);
      }
    });
  }

  private validarFormulario(): boolean {
    if (this.mantenimiento.atraccionId === 0) {
      alert('Debe seleccionar una atracción');
      return false;
    }
    if (!this.mantenimiento.fechaProgramada) {
      alert('Debe ingresar una fecha');
      return false;
    }
    if (!this.mantenimiento.horaInicio) {
      alert('Debe ingresar una hora de inicio');
      return false;
    }
    if (!this.mantenimiento.duracionEstimada) {
      alert('Debe ingresar la duración estimada');
      return false;
    }
    if (!this.mantenimiento.descripcion.trim()) {
      alert('Debe ingresar una descripción');
      return false;
    }
    return true;
  }

  private convertirDuracionATimeSpan(horas: string): string {
    const horasNum = parseFloat(horas);
    const horasEnteras = Math.floor(horasNum);
    const minutos = Math.round((horasNum - horasEnteras) * 60);
    return `${horasEnteras.toString().padStart(2, '0')}:${minutos.toString().padStart(2, '0')}:00`;
  }
}
