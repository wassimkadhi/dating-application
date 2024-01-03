import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import {
  FormBuilder,
  FormControl,
  FormControlName,
  FormGroup,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { getTime } from 'ngx-bootstrap/chronos/utils/date-getters';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent implements OnInit {
  constructor(
    private accounstService: AccountService,
    private toast: ToastrService,
    private fb: FormBuilder,
    private router :Router 
  ) {}
  model: any = {};
  @Output() cancelRegister = new EventEmitter();
  registerForm: FormGroup;
  maxDate: Date = new Date();

  ngOnInit(): void {
   
    this.initializeForm();
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18);
  }



  initializeForm(){
    this.registerForm = this.fb.group({
      gender: ['male', Validators.required],
      knownAs: [null, Validators.required],
      city: [null, Validators.required],
      country: [null, Validators.required],
      DateOfBirth: [null, Validators.required],
      username: [null, Validators.required],
      password: [
        null,
        [Validators.required, Validators.minLength(4), Validators.maxLength(8)],
      ],
      confirmPassword: [null, Validators.required],
    });

  }

  register() {
    const dob =this.getDateOnly(this.registerForm.controls['DateOfBirth'].value);
    const values={...this.registerForm.value, DateOfBirth:dob};
    this.accounstService.register(values).subscribe({
      next: () => {
        this.router.navigateByUrl('/members')
        
      },
      error: (error) => this.toast.error(error.error),
    });
  }

  cancel() {
    this.cancelRegister.emit(false);
  }

  private getDateOnly(dob: string | undefined ) {
    if(!dob) return ; 
    let theDob=new Date(dob) ; 
    return new Date(theDob.setMinutes(theDob.getMinutes()-theDob.getTimezoneOffset())).toISOString().slice(0,10) ;
  }
}
