import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { CuentaService } from '../../../core/services/cuenta.service';
import { CuentaDto, Rol } from '../../../core/models/cuenta.model';
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
  successMessage = '';
  cuentaSeleccionada: CuentaDto | null = null;
  mostrarModal = false;
  mostrarModalRoles = false;
  cuentaParaRoles: CuentaDto | null = null;

  rolesDisponibles = [
    { value: Rol.Administrador, label: 'Administrador' },
    { value: Rol.Operador, label: 'Operador' },
    { value: Rol.Visitante, label: 'Visitante' }
  ];

  ngOnInit(): void {
    this.cargarCuentas();
  }

  cargarCuentas(): void {
    this.loading = true;
    this.errorMessage = '';
    const cuentaIdActual = this.cuentaParaRoles?.id;
    
    this.cuentaService.obtenerCuentas().subscribe({
      next: (response) => {
        this.cuentas = response.content;
        this.loading = false;
        
        if (cuentaIdActual) {
          this.cuentaParaRoles = this.cuentas.find(c => c.id === cuentaIdActual) || null;
        }
      },
      error: (error) => {
        console.error('Error al cargar cuentas:', error);
        this.errorMessage = 'Error al cargar las cuentas';
        this.loading = false;
      }
    });
  }

  abrirModalMembresia(cuenta: CuentaDto): void {
    this.cuentaSeleccionada = cuenta;
    this.mostrarModal = true;
  }

  cerrarModal(): void {
    this.mostrarModal = false;
    this.cuentaSeleccionada = null;
  }

  onMembresiaCambiada(): void {
    this.cerrarModal();
    this.cargarCuentas();
  }

  abrirModalRoles(cuenta: CuentaDto): void {
    this.cuentaParaRoles = cuenta;
    this.mostrarModalRoles = true;
  }

  cerrarModalRoles(): void {
    this.mostrarModalRoles = false;
    this.cuentaParaRoles = null;
  }

  tieneRol(cuenta: CuentaDto, rolLabel: string): boolean {
    return cuenta.roles.includes(rolLabel);
  }

  agregarRol(cuenta: CuentaDto, rol: number): void {
    this.errorMessage = '';
    this.successMessage = '';

    this.cuentaService.agregarRol(cuenta.id, rol).subscribe({
      next: (response) => {
        this.successMessage = response.message || 'Rol agregado exitosamente';
        this.cargarCuentas();
        setTimeout(() => this.successMessage = '', 3000);
      },
      error: (error) => {
        console.error('Error al agregar rol:', error);
        this.errorMessage = error.error?.message || 'Error al agregar rol';
      }
    });
  }

  quitarRol(cuenta: CuentaDto, rol: number): void {
    if (!confirm('¿Está seguro de quitar este rol?')) {
      return;
    }

    this.errorMessage = '';
    this.successMessage = '';

    this.cuentaService.quitarRol(cuenta.id, rol).subscribe({
      next: (response) => {
        this.successMessage = response.message || 'Rol eliminado exitosamente';
        this.cargarCuentas();
        setTimeout(() => this.successMessage = '', 3000);
      },
      error: (error) => {
        console.error('Error al quitar rol:', error);
        this.errorMessage = error.error?.message || 'Error al quitar rol';
      }
    });
  }

  esVisitante(cuenta: CuentaDto): boolean {
    return cuenta.roles.includes('Visitante');
  }

  getNivelMembresiaTexto(nivel: string): string {
    return nivel || 'Sin membresía';
  }

  getNivelMembresiaClass(nivel: string): string {
    return nivel?.toLowerCase() || '';
  }
}