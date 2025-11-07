import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { CuentaService } from '../../../core/services/cuenta.service';

@Component({
  selector: 'app-registro-visitante',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './registro-visitante.html',
  styleUrls: ['./registro-visitante.css']
})
export class RegistroVisitanteComponent {
  private fb = inject(FormBuilder);
  private cuentaService = inject(CuentaService);
  private router = inject(Router);

  registroForm: FormGroup;
  loading = false;
  errorMessage = '';
  successMessage = '';

  constructor() {
    this.registroForm = this.fb.group({
      nombre: ['', [Validators.required, Validators.minLength(2)]],
      apellido: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', Validators.required]
    }, {
      validators: this.passwordMatchValidator
    });
  }

  passwordMatchValidator(form: FormGroup) {
    const password = form.get('password');
    const confirmPassword = form.get('confirmPassword');
    
    if (password && confirmPassword && password.value !== confirmPassword.value) {
      confirmPassword.setErrors({ passwordMismatch: true });
      return { passwordMismatch: true };
    }
    return null;
  }

  get f() {
    return this.registroForm.controls;
  }

  onSubmit(): void {
    this.errorMessage = '';
    this.successMessage = '';

    if (this.registroForm.invalid) {
      Object.keys(this.registroForm.controls).forEach(key => {
        this.registroForm.get(key)?.markAsTouched();
      });
      return;
    }

    this.loading = true;

    const { confirmPassword, ...dto } = this.registroForm.value;

    this.cuentaService.registrarVisitante(dto).subscribe({
      next: (response) => {
        console.log('Registro exitoso:', response);
        this.successMessage = response.message;
        
        setTimeout(() => {
          this.router.navigate(['/login']);
        }, 2000);
      },
      error: (error) => {
        console.error('Error en registro:', error);
        this.errorMessage = error.error?.message || 'Error al registrar visitante';
        this.loading = false;
      }
    });
  }

  irAlLogin(): void {
    this.router.navigate(['/login']);
  }
}
