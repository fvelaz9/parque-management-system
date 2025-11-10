import { CuentaDto } from './cuenta.model';

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  content: {
    token: string;
    cuenta: CuentaDto;
  };
  executionSuccessful: boolean;
  message: string;
}

export interface VisitanteDto {
  id: string;
  fechaNacimiento: Date;
  edad: number;
  nivelMembresia: string;
  puntosDiarios: number;
  puntosTotales: number;
}

export type { CuentaDto };
