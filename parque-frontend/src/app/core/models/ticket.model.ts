export interface Ticket {
  id: number;
  codigo: string;
  fechaVisita: string;
  tipoEntrada: TipoTicket;
  cuentaId: number;
  eventoId?: number;
  estado?: string;
}

export enum TipoTicket {
  General = 0,
  EventoEspecial = 1
}

export interface CrearTicketDto {
  fechaVisita: string;
  tipoEntrada: TipoTicket;
  eventoId?: number;
}