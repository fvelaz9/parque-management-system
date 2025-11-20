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

  isMantenimientoDropdownOpen = false; 
  isTicketsDropdownOpen = false; 
  isAdminDropdownOpen = false;

  get isLoggedIn(): boolean {
    return this.authService.isLoggedIn();
  }

  get usuario() {
    return this.authService.getUsuario();
  }

  get isAdmin(): boolean {
    return this.authService.getUsuario()?.roles?.includes('Administrador') || false;
  }
  get isOperador(): boolean {
    return this.authService.getUsuario()?.roles?.includes('Operador') || false;
  }
  get isVisitante(): boolean {
    return this.authService.getUsuario()?.roles?.includes('Visitante') || false;
  }

  toggleAdminDropdown(event: Event): void {
    event.stopPropagation();
    this.isAdminDropdownOpen = !this.isAdminDropdownOpen;
    this.isMantenimientoDropdownOpen = false;
    this.isTicketsDropdownOpen = false;
  }

  toggleMantenimientoDropdown(event: Event): void {
    event.stopPropagation();
    this.isMantenimientoDropdownOpen = !this.isMantenimientoDropdownOpen;
    this.isTicketsDropdownOpen = false;
    this.isAdminDropdownOpen = false;
  }

  toggleTicketsDropdown(event: Event): void {
    event.stopPropagation();
    this.isTicketsDropdownOpen = !this.isTicketsDropdownOpen;
    this.isMantenimientoDropdownOpen = false;
    this.isAdminDropdownOpen = false;
  }

  onClickOutside(event: Event): void {
    this.isMantenimientoDropdownOpen = false;
    this.isTicketsDropdownOpen = false;
    this.isAdminDropdownOpen = false;
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
