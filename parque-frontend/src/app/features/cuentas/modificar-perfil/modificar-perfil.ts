import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { CuentaService } from '../../../core/services/cuenta.service';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-modificar-perfil',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './modificar-perfil.html',
  styleUrls: ['./modificar-perfil.css']
})
export class ModificarPerfilComponent implements OnInit {
  private fb = inject(FormBuilder);
  private cuentaService = inject(CuentaService);
  private authService = inject(AuthService);
  private router = inject(Router);

  perfilForm: FormGroup;
  loading = false;
  errorMessage = '';
  successMessage = '';
  cambiarPassword = false;

  constructor() {
    this.perfilForm = this.fb.group({
      nombre: ['', [Validators.minLength(2)]],
      apellido: ['', [Validators.minLength(2)]],
      email: ['', [Validators.email]],
      password: ['']
    });
  }

  ngOnInit(): void {
    // Verificar que el usuario es visitante
    if (!this.esVisitante()) {
      this.router.navigate(['/home']);
      return;
    }
    this.cargarDatosUsuario();
  }

  get f() {
    return this.perfilForm.controls;
  }

  private esVisitante(): boolean {
    return this.authService.tieneRol('Visitante');
  }

  private cargarDatosUsuario(): void {
    const usuario = this.authService.getUsuario();
    if (usuario) {
      this.perfilForm.patchValue({
        nombre: usuario.nombre,
        apellido: usuario.apellido,
        email: usuario.email
      });
    }
  }

  toggleCambiarPassword(): void {
    this.cambiarPassword = !this.cambiarPassword;
    
    if (this.cambiarPassword) {
      this.f['password'].setValidators([Validators.required, Validators.minLength(6)]);
    } else {
      this.f['password'].clearValidators();
      this.f['password'].setValue('');
    }
    
    this.f['password'].updateValueAndValidity();
  }

  onSubmit(): void {
    this.errorMessage = '';
    this.successMessage = '';

    if (this.perfilForm.invalid) {
      this.markFormAsTouched();
      return;
    }

    const dto = this.buildModificarPerfilDto();

    if (Object.keys(dto).length === 0) {
      this.errorMessage = 'No hay cambios para guardar';
      return;
    }

    this.loading = true;

    console.log('📤 DTO a enviar:', dto);

    this.cuentaService.modificarPerfil(dto).subscribe({
      next: (response) => this.handleSuccess(response, dto),
      error: (error) => this.handleError(error)
    });
  }

  private markFormAsTouched(): void {
    Object.keys(this.perfilForm.controls).forEach(key => {
      this.perfilForm.get(key)?.markAsTouched();
    });
  }

  private buildModificarPerfilDto(): any {
    const dto: any = {};
    const formValue = this.perfilForm.value;
    const usuarioActual = this.authService.getUsuario();

    if (formValue.nombre && formValue.nombre !== usuarioActual?.nombre) {
      dto.nombre = formValue.nombre;
    }
    if (formValue.apellido && formValue.apellido !== usuarioActual?.apellido) {
      dto.apellido = formValue.apellido;
    }
    if (formValue.email && formValue.email !== usuarioActual?.email) {
      dto.email = formValue.email;
    }
    if (this.cambiarPassword && formValue.password) {
      dto.password = formValue.password;
    }

    return dto;
  }

  private handleSuccess(response: any, dto: any): void {
    console.log('✅ Perfil modificado:', response);
    this.successMessage = response.message || 'Perfil actualizado exitosamente';
    
    this.actualizarUsuarioEnLocalStorage(dto);
    
    this.loading = false;
    this.cambiarPassword = false;
    this.f['password'].setValue('');
    this.f['password'].clearValidators();
    this.f['password'].updateValueAndValidity();
  }

  private actualizarUsuarioEnLocalStorage(dto: any): void {
    const usuario = this.authService.getUsuario();
    if (usuario) {
      if (dto.nombre) usuario.nombre = dto.nombre;
      if (dto.apellido) usuario.apellido = dto.apellido;
      if (dto.email) usuario.email = dto.email;
      
      localStorage.setItem('usuario', JSON.stringify(usuario));
    }
  }

  private handleError(error: any): void {
    console.error('❌ Error al modificar perfil:', error);
    
    if (error.status === 401) {
      this.errorMessage = 'No estás autenticado. Por favor inicia sesión.';
      setTimeout(() => {
        this.router.navigate(['/login']);
      }, 2000);
    } else if (error.status === 403) {
      this.errorMessage = 'No tienes permisos para modificar este perfil.';
    } else if (error.status === 400) {
      this.errorMessage = error.error?.message || 'Datos inválidos. Verifica el formulario.';
    } else {
      this.errorMessage = error.error?.message || 'Error al modificar perfil';
    }
    
    this.loading = false;
  }
}
