import {CuentaDto} from './cuenta.model';

export interface RegistrarIngresoRequest {
  codigoTicket: string;
  cuentaVisitanteId: string;
}

export interface AforoResponse {
  atraccionId: number;
  nombreAtraccion: string;
  capacidadTotal: number;
  visitantesActuales: number;
  capacidadRestante: number;
  porcentajeOcupacion: number;
  aforoCompleto: boolean;
}
export interface RegistrarEgresoRequest {
  codigoTicket: string;
}

export interface RegistrarEgresoResponse {
  mensaje: string;
  registro: RegistroVisitaDto;
  tiempoVisitaMinutos: number;
}

export interface RegistroVisitaDto {
  id: number;
  atraccionId: number;
  identificador: string;
  fechaIngreso: Date;
  fechaEgreso?: Date;
}
