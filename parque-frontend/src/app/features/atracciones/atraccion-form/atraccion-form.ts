import { Component } from '@angular/core';
import {FormsModule} from '@angular/forms';

@Component({
  selector: 'app-atraccion-form',
  templateUrl: './atraccion-form.html',
  styleUrls: ['./atraccion-form.css'],
  imports: [
    FormsModule
  ],
  standalone: true
})
export class AtraccionForm {
  atraccion = {
    nombre: '',
    tipo: '',
    edadMinima: 0,
    capacidad: 1,
    descripcion: ''
  };
  tiposDisponibles = ['Montaña Rusa', 'Tobogán', 'Casa Embrujada', 'Carrusel'];

  onSubmit() {
    console.log('Enviando atracción:', this.atraccion);
  }
}



