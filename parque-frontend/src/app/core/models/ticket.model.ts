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
  General = 'General',
  EventoEspecial = 'EventoEspecial'
}

export interface CrearTicketDto {
  fechaVisita: string;
  tipoEntrada: TipoTicket;
  eventoId?: number;
}