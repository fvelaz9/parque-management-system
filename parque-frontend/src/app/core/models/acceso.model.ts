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
