// src/app/core/models/recompensa.model.ts
export interface Recompensa {
  id: string;
  nombre: string;
  descripcion: string;
  costoEnPuntos: number;
  cantidadDisponible: number;
  nivelMembresiaRequerido?: 'Estandar' | 'Premium' | 'VIP';
  fechaCreacion: string;
}

export interface RecompensaListadoResponse {
  total: number;
  recompensas: Recompensa[];
}
