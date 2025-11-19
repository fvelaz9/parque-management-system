import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-acceso-denegado',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './acceso-denegado.html',
  styleUrl: './acceso-denegado.css',
})
export class AccesoDenegado {
  constructor(private router: Router) {}

  irAlHome(): void {
    this.router.navigate(['/home']);
  }

  navegarAtras(): void {
    window.history.back();
  }
}
