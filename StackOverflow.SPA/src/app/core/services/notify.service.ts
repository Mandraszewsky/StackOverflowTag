import { inject, Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({ providedIn: 'root' })
export class NotifyService {
  private readonly snackBar = inject(MatSnackBar);

  success(message: string): void {
    this.snackBar.open(message, 'OK', {
      duration: 3000,
      panelClass: 'snackbar-success',
    });
  }

  error(message: string): void {
    this.snackBar.open(message, 'OK', {
      duration: 5000,
      panelClass: 'snackbar-error',
    });
  }
}
