// src/app/features/mantenimientos/mantenimiento-edit/mantenimiento-edit.component.ts
import { Component, effect, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MantenimientosService } from '../../../core/services/mantenimiento.service';
import { MantenimientoPreventivo } from '../../../core/models/mantenimiento.model';

@Component({
  selector: 'app-mantenimiento-edit',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './mantenimiento-edit.html',
  styleUrl: './mantenimiento-edit.css'
})
export class MantenimientoEdit {
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly mantenimientosService = inject(MantenimientosService);

  mantenimientoForm!: FormGroup;
  loading = signal(true);
  error = signal('');
  mantenimientoId: number = 0;

  private readonly loadEffect = effect(() => {
    this.mantenimientoId = Number(this.route.snapshot.paramMap.get('id'));
    if (this.mantenimientoId) {
      this.cargarMantenimiento();
    }
  });

  constructor() {
    this.inicializarFormulario();
  }

  private inicializarFormulario() {
    this.mantenimientoForm = this.fb.group({
      atraccionId: [0, [Validators.required, Validators.min(1)]],
      fechaProgramada: ['', Validators.required],
      horaInicio: ['', Validators.required],
      duracionEstimada: ['', Validators.required],
      descripcion: ['', [Validators.required]]
    });
  }

  private cargarMantenimiento() {
    this.loading.set(true);
    this.mantenimientosService.getAllMantenimientos().subscribe({
      next: (mantenimientos) => {
        const mantenimiento = mantenimientos.find(m => m.id === this.mantenimientoId);
        if (mantenimiento) {
          this.llenarFormulario(mantenimiento);
        } else {
          this.error.set('Mantenimiento no encontrado');
        }
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error al cargar mantenimiento:', err);
        this.error.set('Error al cargar el mantenimiento');
        this.loading.set(false);
      }
    });
  }

  private llenarFormulario(mantenimiento: MantenimientoPreventivo) {
    this.mantenimientoForm.patchValue({
      atraccionId: mantenimiento.atraccionId,
      fechaProgramada: this.formatearFecha(mantenimiento.fechaProgramada),
      horaInicio: mantenimiento.horaInicio,
      duracionEstimada: mantenimiento.duracionEstimada,
      descripcion: mantenimiento.descripcion
    });
  }

  private formatearFecha(fecha: string): string {
    return fecha.split('T')[0];
  }

  onSubmit() {
    if (this.mantenimientoForm.invalid) {
      alert('Por favor completa todos los campos correctamente');
      return;
    }

    this.mantenimientosService.updateMantenimiento(this.mantenimientoId, this.mantenimientoForm.value)
      .subscribe({
        next: () => {
          alert('Mantenimiento actualizado exitosamente');
          this.router.navigate(['/mantenimientos']);
        },
        error: (err) => {
          console.error('Error al actualizar:', err);
          alert(err.error?.mensaje || 'Error al actualizar el mantenimiento');
        }
      });
  }

  volver() {
    this.router.navigate(['/mantenimientos']);
  }
}
