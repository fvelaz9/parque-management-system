import { Component, inject, signal, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink, ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { CrearRecompensaRequest, NivelMembresia } from '../../../core/models/recompensa.model';
import { RecompensasService } from '../../../core/services/recompensa.service';
@Component({
  selector: 'app-recompensa-editar',
  templateUrl: './recompensa-editar.html',
  styleUrls: ['./recompensa-editar.css'],
  imports: [FormsModule, RouterLink, CommonModule],
  standalone: true
})
export class RecompensaEditar implements OnInit {
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly recompensasService = inject(RecompensasService);

  public submitting = signal(false);
  public loading = signal(true);
  private recompensaId: string = '';

  recompensa: CrearRecompensaRequest = {
    nombre: " ",
    descripcion: " ",
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

  ngOnInit() {
    this.recompensaId = this.route.snapshot.paramMap.get('id') || '';

    if (!this.recompensaId) {
      alert('ID de recompensa no válido');
      this.router.navigate(['/recompensas']);
      return;
    }

    this.cargarRecompensa();
  }

  private cargarRecompensa() {
    this.recompensasService.getById(this.recompensaId).subscribe({
      next: (r) => {
        this.recompensa = {
          nombre: r.nombre,
          descripcion: r.descripcion,
          costoEnPuntos: r.costoEnPuntos,
          cantidadDisponible: r.cantidadDisponible,
          nivelMembresiaRequerido: r.nivelMembresiaRequerido
        };
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error al cargar recompensa:', err);
        alert('Error al cargar la recompensa');
        this.router.navigate(['/recompensas']);
      }
    });
  }

  onSubmit() {
    if (!this.validarFormulario()) {
      return;
    }

    this.submitting.set(true);

    this.recompensasService.update(this.recompensaId, this.recompensa).subscribe({
      next: (res) => {
        console.log('Recompensa actualizada:', res);
        alert('Recompensa actualizada con éxito');
        this.router.navigate(['/recompensas']);
      },
      error: (err) => {
        console.error('Error al actualizar recompensa:', err);
        alert(err.error?.mensaje || 'Error al actualizar la recompensa');
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
