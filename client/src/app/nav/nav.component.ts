import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { User } from '../_models/user';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model:any={} ; 
  user:User
 
  constructor(public accountservice :AccountService ,private router :Router ,private toast : ToastrService ){}

  ngOnInit(): void {
   
    
   
  }

  login(){
    this.accountservice.login(this.model).subscribe({
      next: response =>{
        console.log(response) ; 
        this.router.navigateByUrl('/members') ;
       
       
        } , 

       error:error=>this.toast.error(error.error)
      
     
        


    })
    
  }
  
  logout(){
    this.accountservice.logout();
    this.router.navigateByUrl('/') ;
  }

    
   
}
