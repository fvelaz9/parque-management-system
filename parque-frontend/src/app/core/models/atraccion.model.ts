// src/app/core/models/atraccion.model.ts

export interface AtraccionParque {
  id: number;
  nombre: string;
  tipo: TipoAtraccion;
  edadMinima: number;
  capacidad: number;
  descripcion: string;
  estado: EstadoAtraccion;
}

export enum TipoAtraccion {
  MontañaRusa = 'Montaña Rusa',
  Simulador = 'Simulador',
  Espectaculo = 'Espectaculo',
  ZonaInteractiva = 'Zona Interactiva'
}

export enum EstadoAtraccion {
  Disponible = 'Disponible',
  FueraDeServicio = 'Fuera De Servicio'
}

export interface AforoAtraccionDto {
  atraccionId: number;
  nombreAtraccion: string;
  aforoActual: number;
  capacidadMaxima: number;
  disponible: number;
}

export interface ReporteAtraccionDto {
  atraccionId: number;
  nombreAtraccion: string;
  cantidadVisitas: number;
}
