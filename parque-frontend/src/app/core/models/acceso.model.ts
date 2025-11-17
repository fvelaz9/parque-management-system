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
