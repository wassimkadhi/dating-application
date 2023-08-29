import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { AccountService } from './_services/account.service';
import { ActivatedRoute, Route, Router } from '@angular/router';
import { User } from './_models/user';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'dating site ';
  users :any ;
  loggedin=true;


  constructor(private http :HttpClient , private accountService :AccountService ,private router:Router ,  private route :ActivatedRoute ){


  }
  ngOnInit(): void {
    
    this.setCurrentUser() ;
     
   

  }

  setCurrentUser(){
    const user:User = JSON.parse(localStorage.getItem('user') ) ; 
    this.accountService.setCurrentUser(user)
  }



    
 
}
