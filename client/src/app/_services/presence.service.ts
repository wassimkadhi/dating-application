import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  hubUrl=environment.hubUrl;
  private hubConnection? :HubConnection; 
  private onlineUsersSource = new BehaviorSubject<string[]>([]) ; 
  onlineUsers$=this.onlineUsersSource.asObservable() ;

  constructor(private toaster :ToastrService)  {}




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
      this.toaster.info(username+' has connected');
    })

    this.hubConnection.on('UserIsOffline' ,username=>{
      this.toaster.success(username +' has disconnected');
    })

    this.hubConnection.on('GetOnlinesUsers',usernames=>{
      this.onlineUsersSource.next(usernames) ;
    })

  }

  stopHubConnection(){
    this.hubConnection?.stop().catch(error=>console.log(error)) ;
  }
}
