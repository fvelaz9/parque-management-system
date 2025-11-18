// src/app/core/models/incidencia.model.ts
export interface Incidencia {
  id: number;
  descripcion: string;
  fechaReporte: string;
  fechaResolucionEstimada: string;
  atraccionId: number;
  nombreAtraccion: string;
  estaActiva: boolean;
}
