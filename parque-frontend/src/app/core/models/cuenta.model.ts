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
  rol: number;
  fechaNacimiento?: string | null;
  nivelMembresia?: number | null;
}

export interface ModificarPerfilDto {
  nombre?: string;
  apellido?: string;
  email?: string;
  password?: string;
}

export enum Rol {
  Administrador = 1,
  Operador = 2,
  Visitante = 3
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
  rol?: string;
  visitante: VisitanteDto | null;
}

export interface VisitanteDto {
  id: string;
  fechaNacimiento: string;
  edad: number;
  nivelMembresia: string;
  puntosDiarios: number;
  puntosTotales: number;
  puntos?: number;
}