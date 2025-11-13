// src/app/core/models/recompensa.model.ts

export enum NivelMembresia {
  Estandar = 1,
  Premium = 2,
  VIP = 3
}

export interface Recompensa {
  id: string;
  nombre: string;
  descripcion: string;
  costoEnPuntos: number;
  cantidadDisponible: number;
  nivelMembresiaRequerido?: NivelMembresia;
  fechaCreacion: string;
}
export interface CrearRecompensaRequest {
  Nombre: string;  // ← Mayúscula
  Descripcion?: string;
  CostoEnPuntos: number;
  CantidadDisponible: number;
  NivelMembresiaRequerido?: NivelMembresia;
}

export interface CanjearRecompensaRequest {
  visitanteId: string;
  recompensaId: string;
}

export interface HistorialCanjeDto {
  id: string;
  visitanteId: string;
  recompensaId: string;
  nombreRecompensa: string;
  puntosCanjeados: number;
  fechaCanje: string;
}
