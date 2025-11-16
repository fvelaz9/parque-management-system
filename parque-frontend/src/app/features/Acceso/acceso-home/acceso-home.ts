import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-acceso-home',
  templateUrl: './acceso-home.html',
  styleUrls: ['./acceso-home.css']
})
export class AccesoHome {
  constructor(private router: Router) { }

  irAValidarAcceso() {
    this.router.navigate(['/acceso/validar']);
  }

  irARegistrarIngreso() {
    this.router.navigate(['/acceso/ingreso']);
  }

  irARegistrarEgreso() {
    this.router.navigate(['/acceso/egreso']);
  }

  irAObtenerAforo() {
    this.router.navigate(['/acceso/aforo']);
  }
}
