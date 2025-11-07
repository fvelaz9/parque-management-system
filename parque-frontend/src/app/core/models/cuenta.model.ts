export interface RegistrarVisitanteDto {
  nombre: string;
  apellido: string;
  email: string;
  password: string;
}

export interface RegistrarCuentaDto {
  nombre: string;
  apellido: string;
  email: string;
  password: string;
  roles: string[];
}

export interface ModificarPerfilDto {
  nombre?: string;
  apellido?: string;
  email?: string;
  password?: string;
}

export enum NivelMembresia {
  Estandar = 1,
  Premium = 2,
  VIP = 3
}

export interface ResponseDto<T = any> {
  content: T;
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
  puntos?: number;
  nivelMembresia?: NivelMembresia;
}