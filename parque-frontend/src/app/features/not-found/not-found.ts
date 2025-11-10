import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-not-found',
  imports: [],
  templateUrl: './not-found.html',
  styleUrl: './not-found.css',
})
export class NotFoundComponent {
  private router = inject(Router);

  volverInicio(): void {
    this.router.navigate(['/home']);
  }

  volverAtras(): void {
    window.history.back();
  }
}
