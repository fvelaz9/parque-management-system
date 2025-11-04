import {Component, inject} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {RouterLink} from '@angular/router';
import {NgForOf} from '@angular/common';
import {AtraccionesService} from '../../../core/services/atracciones.service';

@Component({
  selector: 'app-atraccion-edit',
  imports: [FormsModule, RouterLink, NgForOf],
  templateUrl: './atraccion-edit.html',
  styleUrl: './atraccion-edit.css',
  standalone: true
})
export class AtraccionEdit {
  private readonly atraccionesService = inject(AtraccionesService);

}
