import { Component, Input, Output, EventEmitter, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CuentaService } from '../../../core/services/cuenta.service';
import { CuentaDto, NivelMembresia } from '../../../core/models/cuenta.model';

@Component({
  selector: 'app-cambiar-membresia-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './cambiar-membresia-modal.html',
  styleUrls: ['./cambiar-membresia-modal.css']
})
export class CambiarMembresiaModalComponent {
  private cuentaService = inject(CuentaService);

  @Input() cuenta!: CuentaDto;
  @Output() cerrar = new EventEmitter<void>();
  @Output() membresiaCambiada = new EventEmitter<void>();

  nuevoNivel: NivelMembresia = NivelMembresia.Estandar;
  loading = false;
  errorMessage = '';
  successMessage = '';

  nivelesMembresia = [
    { value: NivelMembresia.Estandar, label: 'Estandar' },
    { value: NivelMembresia.Premium, label: 'Premium' },
    { value: NivelMembresia.VIP, label: 'VIP' }
  ];

  ngOnInit(): void {
    if (this.cuenta.visitante?.nivelMembresia) {
      this.nuevoNivel = this.cuenta.visitante.nivelMembresia;
    }
  }

  cambiarMembresia(): void {
    this.loading = true;
    this.errorMessage = '';

    this.cuentaService.cambiarNivelMembresia(this.cuenta.id, this.nuevoNivel).subscribe({
      next: (response) => {
        this.successMessage = response.message;
        this.loading = false;
        
        setTimeout(() => {
          this.membresiaCambiada.emit();
        }, 1500);
      },
      error: (error) => {
        console.error('Error al cambiar membresía:', error);
        this.errorMessage = error.error?.message || 'Error al cambiar la membresía';
        this.loading = false;
      }
    });
  }

  cerrarModal(): void {
    this.cerrar.emit();
  }
}