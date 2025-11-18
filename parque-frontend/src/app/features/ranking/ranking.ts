// src/app/features/ranking/ranking.component.ts
import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment.development';

interface RankingVisitante {
  visitanteId: string;
  nombreVisitante: string;
  puntosTotales: number;
  posicion: number;
}

@Component({
  selector: 'app-ranking',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './ranking.html',
  styleUrl: './ranking.css'
})
export class RankingComponent {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = environment.apiUrl;

  public rankingVisitantes = signal<RankingVisitante[]>([]);
  public loading = signal(false);
  public error = signal('');
  public successMessage = signal('');
  public fechaRanking: string = new Date().toISOString().split('T')[0];
  public topRanking: number = 10;

  // ✅ QUITAR ngOnInit - No cargar nada al iniciar

  cargarRanking() {
    this.loading.set(true);
    this.error.set('');
    this.successMessage.set(''); // Limpiar mensajes previos

    const fecha = this.fechaRanking || new Date().toISOString().split('T')[0];
    const url = `${this.apiUrl}/gamificacion/ranking/diario?fecha=${fecha}&top=${this.topRanking}`;

    this.http.get<any>(url).subscribe({
      next: (response) => {
        console.log('Ranking response:', response);
        const ranking = response.content?.ranking || response.ranking || [];
        this.rankingVisitantes.set(ranking);

        // ✅ Mostrar mensaje de éxito
        const mensaje = response.message || `Ranking cargado correctamente (${ranking.length} visitantes)`;
        this.successMessage.set(mensaje);

        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error al cargar ranking:', err);
        this.error.set('Error al cargar el ranking diario');
        this.loading.set(false);
      }
    });
  }
}
