import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { User } from '../_models/user';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model:any={} ; 
  user:User
 
  constructor(public accountservice :AccountService){}

  ngOnInit(): void {
   
    
   
  }

  login(){
    this.accountservice.login(this.model).subscribe({
      next: response =>{
        console.log(response) ; 
       
       
        } , 

      error:error=>console.log(error)  
     
        


    })
    
  }
  
  logout(){
    this.accountservice.logout(); 
  }

    
   
}
