import { HttpClient, JsonpInterceptor } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';
import { User } from '../_models/user';
import { environment } from 'src/environments/environment';
import { PresenceService } from './presence.service';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  baseUrl = environment.apiUrl;

  //to storage user information in an observables so aother componenet kwnow about ouer login status
  private currentUserSource = new BehaviorSubject<User | null>(null);
  // we can use outsude the account service create  an observables
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient,private presenceService :PresenceService) {}

  login(model: any) {
    return this.http.post<User>(this.baseUrl + 'account/login', model).pipe(
      map((response: User) => {
        const user = response;
        if (user) {
          this.setCurrentUser(user)
        }
      
      })
    );
  }

  register(model: any) {
    return this.http.post<User>(this.baseUrl + 'account/register', model).pipe(
      map((user) => {
        if (user) {
          this.setCurrentUser(user) ;
        }
      })
    );
  }

  setCurrentUser(user: User) {
    user.roles=[] ;
    const roles=this.getDecodedToken(user.token).role ;
    Array.isArray(roles)? user.roles=roles:user.roles.push(roles);
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSource.next(user);
    console.log("this is the current" +user) ;
    this.presenceService.createHubConnection(user);
  }

  logout() {
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
    this.presenceService.stopHubConnection() ;
  }



  getDecodedToken(token :string){ 

    return JSON.parse(atob(token.split('.')[1]))

  }
}
