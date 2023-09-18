import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent implements OnInit {
  constructor(
    private accounstService: AccountService,
    private toast: ToastrService
  ) {}
  model: any = {};
  @Input() usersfromhomecomponet: any;
  @Output() cancelRegister = new EventEmitter();

  ngOnInit(): void {}

  register() {
    this.accounstService.register(this.model).subscribe({
      next: () => {
        this.cancel();
      },
      error: (error) => this.toast.error(error.error),
    });
  }

  cancel() {
    this.cancelRegister.emit(false);
  }
}
