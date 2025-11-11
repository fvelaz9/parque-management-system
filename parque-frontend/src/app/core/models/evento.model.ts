

export interface Evento {
  id: number;
  titulo: string;
  descripcion: string;
  inicio: string;
  fin: string;
  aforoMaximo: number;
  costoAdicional: number;
  estado: EstadoEvento;

}
export enum EstadoEvento {
  Programado = 1,
  Activo = 2,
  Finalizado = 3,
  Cancelado = 4
}
