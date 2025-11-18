import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AccesoService } from '../../../core/services/acceso.service';
import { AforoResponse } from '../../../core/models/acceso.model';

@Component({
  selector: 'app-acceso-home',
  templateUrl: './acceso-home.html',
  styleUrls: ['./acceso-home.css'],
  standalone: true,
  imports: [CommonModule, RouterLink]
})
export class AccesoHome implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private accesoService = inject(AccesoService);

  atraccionId: number = 0;
  atraccion: any = null;
  aforo: AforoResponse | null = null;
  loading: boolean = false;

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.atraccionId = +params['atraccionId'] || 0;
      if (this.atraccionId) {
        this.cargarAforo();
      }
    });
  }

  cargarAforo(): void {
    this.loading = true;
    this.accesoService.obtenerAforo(this.atraccionId).subscribe({
      next: (response: AforoResponse) => {
        this.aforo = response;
        this.atraccion = { nombre: `Atracción ${this.atraccionId}` };
        this.loading = false;
      },
      error: (error: any) => {
        console.error('Error al cargar aforo:', error);
        this.loading = false;
      }
    });
  }
}
