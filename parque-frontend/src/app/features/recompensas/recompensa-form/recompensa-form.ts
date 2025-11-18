import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { CrearRecompensaRequest, NivelMembresia } from '../../../core/models/recompensa.model';
import { RecompensasService } from '../../../core/services/recompensa.service';

@Component({
  selector: 'app-recompensa-form',
  templateUrl: './recompensa-form.html',
  styleUrls: ['./recompensa-form.css'],
  imports: [FormsModule, RouterLink, CommonModule],
  standalone: true
})
export class RecompensaForm {
  private readonly router = inject(Router);
  private readonly recompensasService = inject(RecompensasService);

  public submitting = signal(false);

  recompensa: CrearRecompensaRequest = {
    nombre: '',
    descripcion: '',
    costoEnPuntos: 0,
    cantidadDisponible: 0,
    nivelMembresiaRequerido: undefined
  };

  readonly nivelesMembresia = [
    { valor: undefined, etiqueta: 'Sin requisito' },
    { valor: NivelMembresia.Estandar, etiqueta: 'Estandar' },
    { valor: NivelMembresia.Premium, etiqueta: 'Premium' },
    { valor: NivelMembresia.VIP, etiqueta: 'VIP' }
  ];

  onSubmit() {
    if (!this.validarFormulario()) {
      return;
    }

    this.submitting.set(true);
    console.log('Enviando recompensa:', this.recompensa);

    this.recompensasService.create(this.recompensa).subscribe({
      next: (res) => {
        console.log('Recompensa creada:', res);
        alert('Recompensa creada con éxito');
        this.router.navigate(['/recompensas']);
      },
      error: (err) => {
        console.error('Error al crear recompensa:', err);

        const errorMsg = err.error?.message || err.error?.mensaje || 'Error al crear la recompensa';
        alert(errorMsg);

        this.submitting.set(false);
      }
    });
  }


  private validarFormulario(): boolean {
    if (!this.recompensa.nombre.trim()) {
      alert('Debe ingresar un nombre');
      return false;
    }

    if (this.recompensa.costoEnPuntos <= 0) {
      alert('El costo debe ser mayor a 0');
      return false;
    }

    if (this.recompensa.cantidadDisponible < 0) {
      alert('La cantidad no puede ser negativa');
      return false;
    }

    return true;
  }
}
