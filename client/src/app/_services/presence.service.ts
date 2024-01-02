import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { BehaviorSubject, Observable, take } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  hubUrl=environment.hubUrl;
  private hubConnection? :HubConnection; 
  private onlineUsersSource = new BehaviorSubject<string[]>([]) ; 
  onlineUsers$=this.onlineUsersSource.asObservable() ;

  constructor(private toaster :ToastrService, private router:Router)  {}




  createHubConnection(user:User){
    this.hubConnection= new HubConnectionBuilder()
    .withUrl(this.hubUrl+'presence',
    //to pass the usertoken
    {accessTokenFactory:()=>user.token})
    //client if losse connection wil reconect
    .withAutomaticReconnect()
    .build() ; 

    this.hubConnection.start().catch(error=>console.log(error));

    this.hubConnection.on('UserIsOnline',username => {
      this.onlineUsers$.pipe(take(1)).subscribe({
        next:usernames=>this.onlineUsersSource.next([...usernames,username])
      })
    })

    this.hubConnection.on('UserIsOffline' ,username=> {
      this.onlineUsers$.pipe(take(1)).subscribe({
        next:usernames=>this.onlineUsersSource.next(usernames.filter(x=> x!== username))
      })
    })

    this.hubConnection.on('GetOnlinesUsers',usernames=>{
      this.onlineUsersSource.next(usernames) ;
    })


    this.hubConnection.on('newMessageReceived',({username,knownAs}) => {
      this.toaster.info(knownAs+' has sent u a message click to see it ').onTap.
      pipe(take(1)).subscribe({
        next:()=>this.router.navigateByUrl('/members/details/'+username+'?Tab=Messages')
      })
      ;
    })

  }

  stopHubConnection(){
    this.hubConnection?.stop().catch(error=>console.log(error)) ;
  }
}
