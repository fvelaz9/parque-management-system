import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { CuentaService } from '../../../core/services/cuenta.service';
import { CuentaDto, NivelMembresia } from '../../../core/models/cuenta.model';
import { CambiarMembresiaModalComponent } from "../cambiar-membresia-modal/cambiar-membresia-modal";

@Component({
  selector: 'app-lista-cuentas',
  standalone: true,
  imports: [CommonModule, RouterLink, CambiarMembresiaModalComponent],
  templateUrl: './lista-cuentas.html',
  styleUrls: ['./lista-cuentas.css']
})
export class ListaCuentasComponent implements OnInit {
  private cuentaService = inject(CuentaService);
  private router = inject(Router);

  cuentas: CuentaDto[] = [];
  loading = false;
  errorMessage = '';
  cuentaSeleccionada: CuentaDto | null = null;
  mostrarModal = false;

  ngOnInit(): void {
    this.cargarCuentas();
  }

  cargarCuentas(): void {
    this.loading = true;
    this.cuentaService.obtenerCuentas().subscribe({
      next: (response) => {
        this.cuentas = response.content;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error al cargar cuentas:', error);
        this.errorMessage = 'Error al cargar las cuentas';
        this.loading = false;
      }
    });
  }

  abrirModalMembresia(cuenta: CuentaDto): void {
    if (!cuenta.visitante) {
      alert('Solo los visitantes tienen membresía');
      return;
    }
    this.cuentaSeleccionada = cuenta;
    this.mostrarModal = true;
  }

  cerrarModal(): void {
    this.mostrarModal = false;
    this.cuentaSeleccionada = null;
  }

  onMembresiaCambiada(): void {
    this.cerrarModal();
    this.cargarCuentas(); // Recargar lista
  }

  esVisitante(cuenta: CuentaDto): boolean {
    return cuenta.roles.includes('Visitante');
  }

  getNivelMembresiaTexto(nivel: number): string {
    const niveles: { [key: number]: string } = {
      1: 'Bronce',
      2: 'Plata',
      3: 'Oro'
    };
    return niveles[nivel] || 'Sin membresía';
  }

  getNivelMembresiaClass(nivel: number): string {
    const classes: { [key: number]: string } = {
      1: 'bronce',
      2: 'plata',
      3: 'oro'
    };
    return classes[nivel] || '';
  }
}