import { Component, OnInit } from '@angular/core';
import {Router, ActivatedRoute, RouterLink} from '@angular/router';
import {CuentaDto} from '../../../core/models/cuenta.model';
import {Ticket, TipoTicket} from '../../../core/models/ticket.model';
import {TicketService} from '../../../core/services/ticket.service';
import {AccesoService} from '../../../core/services/acceso.service';
import {RegistrarIngresoRequest} from '../../../core/models/acceso.model';
import {CuentaService} from '../../../core/services/cuenta.service';
import {CommonModule} from '@angular/common';

@Component({
  selector: 'app-acceso-ingreso',
  templateUrl: './acceso-ingreso.html',
  styleUrls: ['./acceso-ingreso.css'],
  standalone: true,
  imports: [CommonModule, RouterLink]
})
export class AccesoIngreso implements OnInit {

  usuarios: CuentaDto[] = [];
  usuarioSeleccionado: CuentaDto | null = null;
  ticketsUsuario: Ticket[] = [];
  ticketSeleccionado: Ticket | null = null;
  mensajeRespuesta = '';
  loading: boolean = false;
  errorMessage: string = '';
  atraccionId: number = 0;
  eventoId?: number;

  constructor(
    private ticketService: TicketService,
    private accesoService: AccesoService,
    private cuentaService: CuentaService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.atraccionId = +params['atraccionId'] || 0;
      this.eventoId = +params['eventoId'] || undefined;
    });
    this.cargarUsuarios();
  }
  cargarUsuarios(): void {
    this.loading = true;
    this.cuentaService.obtenerCuentasVisitantes().subscribe({
      next: (response) => {
        this.usuarios = response.content;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error al cargar cuentas:', error);
        this.errorMessage = 'Error al cargar las cuentas';
        this.loading = false;
      }
    });
  }
  seleccionarUsuario(usuario: CuentaDto): void {
    this.usuarioSeleccionado = usuario;
    this.ticketSeleccionado = null;
    this.cargarTickets(usuario.id);
  }

  cargarTickets(usuarioId: string): void {
    if (this.eventoId) {
      this.ticketService.getTicketsPorUsuarioYEvento(usuarioId, this.eventoId).subscribe({
        next: (tickets) => {
          this.ticketsUsuario = tickets;
          this.mensajeRespuesta = '';
        },
        error: (error) => {
          console.error('Error cargando tickets del evento:', error);
          this.ticketsUsuario = [];
          this.mensajeRespuesta = error.error?.message || 'Error cargando tickets del evento';
        }
      });
    } else {
      this.ticketService.getTicketsPorUsuario(usuarioId).subscribe({
        next: (tickets) => {
          this.ticketsUsuario = tickets;
          this.mensajeRespuesta = '';
        },
        error: () => {
          this.ticketsUsuario = [];
          this.mensajeRespuesta = 'Error cargando tickets';
        }
      });
    }
  }

  registrarIngreso(): void {
    if (!this.ticketSeleccionado || !this.usuarioSeleccionado) {
      this.mensajeRespuesta = 'Por favor selecciona ticket y usuario';
      return;
    }
    if (this.eventoId && this.ticketSeleccionado.tipoEntrada !== 1) {
      this.mensajeRespuesta = 'Para acceso a evento se requiere ticket de tipo Evento Especial';
      return;
    }

    if (!this.eventoId && this.ticketSeleccionado.tipoEntrada === 1) {
      this.mensajeRespuesta = 'Ticket de Evento Especial no válido para acceso general';
      return;
    }

    const payload: RegistrarIngresoRequest = {
      codigoTicket: this.ticketSeleccionado.codigo,
      cuentaVisitanteId: this.usuarioSeleccionado.id
    };

    this.accesoService.registrarIngreso(this.atraccionId, payload).subscribe({
      next: (response) => {
        this.mensajeRespuesta = response.mensaje || 'Ingreso registrado exitosamente';
        this.cargarUsuarios();
        setTimeout(() => {
          this.router.navigate(['/atracciones']);
        }, 3000);
      },
      error: (error) => {
        console.error('Error:', error);
        this.mensajeRespuesta = error.error?.message || 'Error al registrar ingreso';
      }
    });
  }

  /*volver(): void {
    this.router.navigate(['/acceso'], { queryParams: { atraccionId: this.atraccionId } });
  }*/
  protected readonly TipoTicket = TipoTicket;
}
