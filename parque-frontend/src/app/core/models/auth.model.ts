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

export interface CuentaDto {
  id: string;
  nombre: string;
  apellido: string;
  email: string;
  roles: string[];
  visitante: VisitanteDto | null;
}

export interface VisitanteDto {
  id: string;
  fechaNacimiento: Date;
  edad: number;
  nivelMembresia: string;
  puntosDiarios: number;
  puntosTotales: number;
}