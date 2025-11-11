import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { CuentaService } from '../../../core/services/cuenta.service';

@Component({
  selector: 'app-crear-cuenta',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './crear-cuenta.html',
  styleUrls: ['./crear-cuenta.css']
})
export class CrearCuentaComponent {
  private fb = inject(FormBuilder);
  private cuentaService = inject(CuentaService);
  private router = inject(Router);

  cuentaForm: FormGroup;
  loading = false;
  errorMessage = '';
  successMessage = '';

  rolesDisponibles = [
    { value: 1, label: 'Administrador', nombre: 'Administrador' },
    { value: 2, label: 'Operador', nombre: 'Operador' },
    { value: 3, label: 'Visitante', nombre: 'Visitante' }
  ];

  rolSeleccionado: number | null = null;

  constructor() {
    this.cuentaForm = this.fb.group({
      nombre: ['', [Validators.required, Validators.minLength(2)]],
      apellido: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  get f() {
    return this.cuentaForm.controls;
  }

  toggleRole(rol: number): void {
    this.rolSeleccionado = rol;
  }

  isRoleSelected(rol: number): boolean {
    return this.rolSeleccionado === rol;
  }

  onSubmit(): void {
    this.errorMessage = '';
    this.successMessage = '';

    if (this.cuentaForm.invalid) {
      Object.keys(this.cuentaForm.controls).forEach(key => {
        this.cuentaForm.get(key)?.markAsTouched();
      });
      return;
    }

    if (this.rolSeleccionado === null) {
      this.errorMessage = 'Debe seleccionar un rol';
      return;
    }

    this.loading = true;

    const dto = {
      ...this.cuentaForm.value,
      rol: this.rolSeleccionado,
      fechaNacimiento: null,
      nivelMembresia: null
    };

    this.cuentaService.crearCuenta(dto).subscribe({
      next: (response) => {
        console.log('Cuenta creada:', response);
        this.successMessage = response.message;
        
        this.cuentaForm.reset();
        this.rolSeleccionado = null;
        this.loading = false;

        setTimeout(() => {
          this.router.navigate(['/cuentas']);
        }, 2000);
      },
      error: (error) => {
        console.error('Error al crear cuenta:', error);
        
        if (error.status === 401) {
          this.errorMessage = 'No estás autenticado. Por favor inicia sesión primero.';
        } else if (error.status === 403) {
          this.errorMessage = 'No tienes permisos para crear cuentas.';
        } else {
          this.errorMessage = error.error?.message || 'Error al crear cuenta';
        }
        
        this.loading = false;
      }
    });
  }

  cancelar(): void {
    this.router.navigate(['/cuentas']);
  }
}
