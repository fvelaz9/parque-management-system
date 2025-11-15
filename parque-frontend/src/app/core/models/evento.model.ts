import { AtraccionParque } from './atraccion.model';

export interface Evento {
  id: number;
  titulo: string;
  descripcion: string;
  inicio: string;
  fin: string;
  aforoMaximo: number;
  costoAdicional: number;
  estado: EstadoEvento;
  atracciones: AtraccionParque[];
}
export enum EstadoEvento {
  Programado = 1,
  Activo = 2,
  Finalizado = 3,
  Cancelado = 4
}
export interface CreateEventoRequest {
  titulo: string;
  descripcion: string;
  inicio: string; // fechas como ISO string
  fin: string;
  aforoMaximo: number;
  costoAdicional: number;
  atraccionIds: number[];
  estado: EstadoEvento;
}
export interface EventoOutDto {
  titulo: string;
}
