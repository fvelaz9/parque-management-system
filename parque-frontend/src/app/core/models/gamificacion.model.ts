// src/app/core/models/gamificacion.model.ts

export interface RankingVisitanteDto {
  visitanteId: string;
  nombre: string;
  puntosDiarios: number;
  puntosTotales: number;
  posicion: number;
}

export interface HistorialPuntuacionDto {
  fechaHora: string;
  origenPuntos: string;
  estrategiaActiva: string;
  puntos: number;
}
