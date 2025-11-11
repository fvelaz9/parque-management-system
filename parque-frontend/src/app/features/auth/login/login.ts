import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../../core/services/auth.service';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-login',
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './login.html',
  styleUrls: ['./login.css'],
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private authService = inject(AuthService);

  loginForm: FormGroup;
  loading = false;
  errorMessage = '';

  constructor() {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  get f() {
    return this.loginForm.controls;
  }

  onSubmit(): void {
    this.errorMessage = '';

    if (this.loginForm.invalid) {
      // Marcar todos los campos como "tocados" para mostrar errores
      Object.keys(this.loginForm.controls).forEach(key => {
        this.loginForm.get(key)?.markAsTouched();
      });
      return;
    }

    this.loading = true;

    const loginData = {
      email: this.loginForm.value.email,
      password: this.loginForm.value.password
    };

    // Llamar al servicio de autenticación
    this.authService.login(loginData).subscribe({
      next: (response) => {
        console.log('Login exitoso', response);
        this.loading = false;
        this.router.navigate(['/home']);
      },
      error: (error: HttpErrorResponse) => {
        console.error('Error en login:', error);
        console.error('Status:', error.status);
        console.error('Response:', error.error);
        
        if (error.status === 0) {
          this.errorMessage = 'No se pudo conectar con el servidor. Verifica que el backend esté corriendo.';
        } else if (error.status === 401) {
          this.errorMessage = 'Credenciales incorrectas. Verifica tu email y contraseña.';
        } else if (error.status === 400) {
          this.errorMessage = error.error?.message || 'Datos inválidos. Verifica el formulario.';
        } else {
          this.errorMessage = 'Error inesperado. Intenta nuevamente.';
        }
        
        this.loading = false;
      }
    });
  }
}
