export interface Ticket {
  id: number;
  cuentaId: string;
  fechaVisita: string;
  eventoId?: number | null;
  tipoEntrada: TipoTicket;
  codigo: string;
  fechaEmision: string;
  esValido: boolean;
}

export enum TipoTicket {
  General = 0,
  EventoEspecial = 1
}
