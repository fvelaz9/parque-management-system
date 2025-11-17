export interface ValidarAccesoRequest {
  codigoTicket: string;
  atraccionId: number;
  cuentaVisitanteId: string;
}

export interface ValidarAccesoResponse {
  accesoPermitido: boolean;
  mensaje: string;
  nombreAtraccion?: string;
  nombreVisitante?: string;
}

export interface RegistrarIngresoRequest {
  codigoTicket: string;
  cuentaVisitante: CuentaVisitanteDto;
}

export interface RegistrarIngresoResponse {
  mensaje: string;
  registro: RegistroVisitaDto;
  fechaIngreso: string;
}

export interface CuentaVisitanteDto {
  id: string;
  nombre?: string;
  apellido?: string;
  // agregar campos que necesites
}

export interface RegistroVisitaDto {
  id: string;
  atraccionId: number;
  identificador: string;
  fechaIngreso: string;
  fechaEgreso?: string;
  // otros campos si los necesitas
}

export interface EgresoResponse {
  mensaje: string;
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
