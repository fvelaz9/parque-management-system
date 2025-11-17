import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive],
  templateUrl: './navbar.html',
  styleUrl: './navbar.css',
  host: {
    '(document:click)': 'onClickOutside($event)'
  }
})
export class NavbarComponent {
  private authService = inject(AuthService);
  private router = inject(Router);

  // Estados de los dropdowns
  isDropdownOpen = false;
  isAdminDropdownOpen = false; // ✅ AGREGAR

  get isLoggedIn(): boolean {
    return this.authService.isLoggedIn();
  }

  get usuario() {
    return this.authService.getUsuario();
  }

  // ✅ AGREGAR: Verificar si es administrador
  get isAdmin(): boolean {
    return this.authService.getUsuario()?.roles?.includes('Administrador') || false;
  }

  // Toggle dropdown Mantenimiento (ya existe)
  toggleDropdown(event: Event): void {
    event.stopPropagation();
    this.isDropdownOpen = !this.isDropdownOpen;
    this.isAdminDropdownOpen = false; // ✅ AGREGAR: Cerrar el otro
  }

  // ✅ AGREGAR: Toggle dropdown Admin
  toggleAdminDropdown(event: Event): void {
    event.stopPropagation();
    this.isAdminDropdownOpen = !this.isAdminDropdownOpen;
    this.isDropdownOpen = false; // Cerrar el otro dropdown
  }

  // Cerrar dropdowns cuando se hace click afuera
  onClickOutside(event: Event): void {
    this.isDropdownOpen = false;
    this.isAdminDropdownOpen = false; // ✅ AGREGAR
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
