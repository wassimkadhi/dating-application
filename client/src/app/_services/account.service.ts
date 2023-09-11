import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';
import { User } from '../_models/user';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AccountService { 

  baseUrl=environment.apiUrl

  //to storage user information in an observables so aother componenet kwnow about ouer login status
  private currentUserSource =new BehaviorSubject<User | null > (null) ;  
  // we can use outsude the account service create  an observables 
  currentUser$=this.currentUserSource.asObservable() ; 


  constructor( private http:HttpClient) { }

login(model:any){
  return this.http.post<User>(this.baseUrl+'account/login',model).pipe(
  
    map ((response:User)=>{
    const user= response ; 
      if(user) {
        localStorage.setItem('user', JSON.stringify(user)) ;
        console.log("hii me again from req" +  user.username)
        this.currentUserSource.next(user);
        console.log(response);
      }
      return(user) ;

    }
  )
  )
}

register(model:any){

  return this.http.post<User>(this.baseUrl+'account/register' ,model).pipe(
    map((response:User) =>{
      if(response) {
        localStorage.setItem('user', JSON.stringify(response)) ;
        console.log('hi im comming from register' +response) ; 
        this.currentUserSource.next(response) ; 
      }
      
    })
  )
}

   setCurrentUser(user:User) {
    this.currentUserSource.next(user);
   }

logout(){
  localStorage.removeItem('user') ;  
  this.currentUserSource.next(null);
}

}
