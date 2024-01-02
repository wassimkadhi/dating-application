import { Injectable } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { initialState } from 'ngx-bootstrap/timepicker/reducer/timepicker.reducer';
import { ConfirmDialogComponent } from '../modals/confirm-dialog/confirm-dialog.component';
import { Observable, map } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ConfirmService {
  bsModelRef?: BsModalRef<ConfirmDialogComponent>;

  constructor(private modelservice: BsModalService) {}

  confirm(
    title = 'Confirmation',
    message = 'Are you sure you want to do this',
    btnOkText = 'Ok',
    btnCancelText = 'Cancel'
  ) :Observable<boolean> {
    const config = {
      initialState: { title, message, btnOkText, btnCancelText },
    };

    this.bsModelRef = this.modelservice.show(ConfirmDialogComponent, config);
    return this.bsModelRef.onHide.pipe(
      map(()=>{
        return this.bsModelRef.content.result
      })
    )
  }
}
