import { Component } from '@angular/core';
import {FormsModule, NgForm} from '@angular/forms';
import {ValidarAccesoRequest, ValidarAccesoResponse} from '../../../core/models/acceso.model';
import {AccesoService} from '../../../core/services/acceso.service';
import {CommonModule} from '@angular/common';
import {RouterLink} from '@angular/router';

@Component({
  selector: 'app-acceso-validar',
  templateUrl: './acceso-validar.html',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterLink
  ],
  styleUrls: ['./acceso-validar.css']
})
export class AccesoValidar {
  codigoTicket: string = '';
  atraccionId!: number;
  cuentaVisitanteId: string = '';

  resultado: ValidarAccesoResponse | null = null;
  errorGeneral: string = '';
  cargando: boolean = false;

  constructor(private accesoService: AccesoService) {}

  onValidarAcceso(form: NgForm) {
    this.errorGeneral = '';
    this.resultado = null;

    if (form.invalid) {
      form.control.markAllAsTouched();
      return;
    }

    const request: ValidarAccesoRequest = {
      codigoTicket: this.codigoTicket,
      atraccionId: this.atraccionId,
      cuentaVisitanteId: this.cuentaVisitanteId
    };

    this.cargando = true;

    this.accesoService.validarAcceso(request).subscribe({
      next: (res) => {
        this.resultado = res;
        this.cargando = false;
      },
      error: (err) => {
        this.errorGeneral = err.error?.message || 'Error al validar acceso';
        this.cargando = false;
      }
    });
  }
}
