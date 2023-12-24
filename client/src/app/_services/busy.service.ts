import { Injectable } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';

@Injectable({
  providedIn: 'root',
})
export class BusyService {
  count = 0;

  constructor(private spinnerService: NgxSpinnerService) {}

  busy() {
    this.count++;
    this.spinnerService.show(undefined, {
      bdColor: 'rgba(0, 0, 0, 0.8)',
      color: '#fff',
    });
  }

  idle() {
    this.count--;
    if (this.count <= 0) {
      this.count = 0;
      this.spinnerService.hide();
    }
  }
}
