import { Component, OnInit } from '@angular/core';
import {ActivatedRoute, Router, RouterLink} from '@angular/router';
import { CommonModule } from '@angular/common';
import {CuentaDto, ResponseDto} from '../../../core/models/cuenta.model';
import {RegistrarEgresoRequest, RegistrarEgresoResponse, RegistroVisitaDto} from '../../../core/models/acceso.model';
import {AccesoService} from '../../../core/services/acceso.service';
import {CuentaService} from '../../../core/services/cuenta.service';

@Component({
  selector: 'app-acceso-egreso',
  templateUrl: './acceso-egreso.html',
  styleUrls: ['./acceso-egreso.css'],
  standalone: true,
  imports: [CommonModule]
})
export class AccesoEgreso implements OnInit {
  atraccionId: number = 0;
  usuarios: CuentaDto[] = [];
  registrosActivos: RegistroVisitaDto[] = [];
  usuarioSeleccionado: CuentaDto | null = null;
  registroSeleccionado: RegistroVisitaDto | null = null;
  mensajeRespuesta: string = '';
  tipoMensaje: 'exito' | 'error' | '' = '';
  loading: boolean = false;
  errorMessage: string = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private accesoService: AccesoService,
    private cuentaService : CuentaService,
  ) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.atraccionId = +params['atraccionId'] || 0;
    });
    this.cargarUsuarios();
  }

  cargarUsuarios(): void {
    this.loading = true;
    this.cuentaService.obtenerCuentas().subscribe({
      next: (response: ResponseDto<CuentaDto[]>) => {
        this.usuarios = response.content;
        this.loading = false;
      },
      error: (error: ResponseDto) => {
        console.error('Error al cargar cuentas:', error);
        this.errorMessage = 'Error al cargar las cuentas';
        this.loading = false;
      }
    });
  }

  seleccionarUsuario(usuario: CuentaDto): void {
    this.usuarioSeleccionado = usuario;
    this.registroSeleccionado = null;
    this.mensajeRespuesta = '';
    this.tipoMensaje = '';

    this.accesoService.obtenerRegistrosActivos(usuario.id, this.atraccionId).subscribe({  // ← Pasar atraccionId
      next: (registros: RegistroVisitaDto[]) => {
        this.registrosActivos = registros || [];

        if (this.registrosActivos.length === 0) {
          this.mensajeRespuesta = 'No hay registros activos para este usuario en esta atracción';
          this.tipoMensaje = '';
        }
      },
      error: (error: any) => {
        console.error('Error al cargar registros:', error);
        this.registrosActivos = [];
        this.mensajeRespuesta = error.message || 'Error al cargar registros';
        this.tipoMensaje = 'error';
      }
    });
  }


  registrarEgreso(): void {
    if (!this.registroSeleccionado) {
      this.mensajeRespuesta = 'Por favor selecciona un registro';
      this.tipoMensaje = 'error';
      return;
    }

    const payload: RegistrarEgresoRequest = {
      codigoTicket: this.registroSeleccionado.identificador.toString()
    };

    console.log('Payload egreso:', payload);

    this.accesoService.registrarEgreso(this.atraccionId, payload).subscribe({
      next: (response: ResponseDto<RegistrarEgresoResponse>) => {
        if (response?.content) {
          const data = response.content;
          this.mensajeRespuesta = `${data.mensaje} - Tiempo de visita: ${data.tiempoVisitaMinutos} minutos`;
          this.tipoMensaje = 'exito';

          this.usuarioSeleccionado = null;
          this.registroSeleccionado = null;

          setTimeout(() => {
            this.cargarUsuarios();
          }, 2000);
        } else {
          this.mensajeRespuesta = 'Egreso registrado';
          this.tipoMensaje = 'exito';
        }
      },
      error: (error: ResponseDto) => {
        console.error('Error:', error);
        this.mensajeRespuesta = error.message || 'Error al registrar egreso';
        this.tipoMensaje = 'error';
      }
    });
  }

  volver(): void {
    this.router.navigate(['/atracciones']);
  }
}
