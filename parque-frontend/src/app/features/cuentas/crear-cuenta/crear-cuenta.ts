import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { CuentaService } from '../../../core/services/cuenta.service';
import { Rol, NivelMembresia } from '../../../core/models/cuenta.model';

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

  private readonly NOT_FOUND = -1;
  private readonly NO_ROLES_SELECTED = 0;


  cuentaForm: FormGroup;
  loading = false;
  errorMessage = '';
  successMessage = '';

  rolesDisponibles = [
    { value: Rol.Administrador, label: 'Administrador', nombre: 'Administrador' },
    { value: Rol.Operador, label: 'Operador', nombre: 'Operador' },
    { value: Rol.Visitante, label: 'Visitante', nombre: 'Visitante' }
  ];

  nivelesMembresia = [
    { value: NivelMembresia.Estandar, label: 'Estandar' },
    { value: NivelMembresia.Premium, label: 'Premium' },
    { value: NivelMembresia.VIP, label: 'VIP' }
  ];

  rolesSeleccionados: number[] = [];

  constructor() {
    this.cuentaForm = this.fb.group({
      nombre: ['', [Validators.required, Validators.minLength(2)]],
      apellido: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      fechaNacimiento: [''],
      nivelMembresia: ['']
    });
  }

  get f() {
    return this.cuentaForm.controls;
  }

  get esVisitante(): boolean {
    return this.rolesSeleccionados.includes(Rol.Visitante);
  }

  toggleRole(rol: number): void {
    const index = this.rolesSeleccionados.indexOf(rol);
    
    if (index > this.NOT_FOUND) {
      // Quitar rol
      this.rolesSeleccionados.splice(index, 1);
    } else {
      // Agregar rol
      this.rolesSeleccionados.push(rol);
    }

    if (this.esVisitante) {
      this.f['fechaNacimiento'].setValidators([Validators.required]);
      this.f['nivelMembresia'].setValidators([Validators.required]);
    } else {
      this.f['fechaNacimiento'].clearValidators();
      this.f['nivelMembresia'].clearValidators();
    }

    this.f['fechaNacimiento'].updateValueAndValidity();
    this.f['nivelMembresia'].updateValueAndValidity();
  }

  isRoleSelected(rol: number): boolean {
    return this.rolesSeleccionados.includes(rol);
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

    if (this.rolesSeleccionados.length === 0) {
      this.errorMessage = 'Debe seleccionar al menos un rol';
      return;
    }

    this.loading = true;

    const rolCombinado = this.rolesSeleccionados.reduce((acc, rol) => acc | rol, this.NO_ROLES_SELECTED);

    const dto = {
      nombre: this.cuentaForm.value.nombre,
      apellido: this.cuentaForm.value.apellido,
      email: this.cuentaForm.value.email,
      password: this.cuentaForm.value.password,
      rol: rolCombinado,
      fechaNacimiento: this.esVisitante ? this.cuentaForm.value.fechaNacimiento : null,
      nivelMembresia: this.esVisitante ? parseInt(this.cuentaForm.value.nivelMembresia) : null
    };

    this.cuentaService.crearCuenta(dto).subscribe({
      next: (response) => {
        this.successMessage = response.message || 'Cuenta creada exitosamente';
        
        this.cuentaForm.reset();
        this.rolesSeleccionados = [];
        this.loading = false;

        setTimeout(() => {
          this.router.navigate(['/cuentas']);
        }, 2000);
      },
      error: (error) => {
        console.error('❌ Error al crear cuenta:', error);
        
        if (error.status === 401) {
          this.errorMessage = 'No estás autenticado. Por favor inicia sesión primero.';
        } else if (error.status === 403) {
          this.errorMessage = 'No tienes permisos para crear cuentas.';
        } else if (error.status === 400) {
          this.errorMessage = error.error?.message || 'Datos inválidos. Verifica el formulario.';
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
