// src/app/core/models/mantenimiento.model.ts
export interface MantenimientoPreventivo {
  id: number;
  atraccionId: number;
  fechaProgramada: string;
  horaInicio: string;
  duracionEstimada: string;
  descripcion: string;
  incidenciaId: number;
}

export interface CrearMantenimientoRequest {
  atraccionId: number;
  fechaProgramada: string;
  horaInicio: string;
  duracionEstimada: string;
  descripcion: string;
}
