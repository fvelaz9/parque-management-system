export interface EstrategiaInfo {
  nombre: string;
  descripcion: string;
  origen: 'Base' | 'Plugin';
  esActiva: boolean;
  parametros?: string[];
}

export interface ResponseDto<T = any> {
  content: T;
  executionSuccessful: boolean;
  message: string;
}

export interface CambiarEstrategiaRequest {
  nombreEstrategia: string;
}