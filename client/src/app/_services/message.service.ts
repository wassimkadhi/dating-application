import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Message } from '../_models/message';
import { environment } from 'src/environments/environment';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { User } from '../_models/user';
import { BehaviorSubject, take } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  baseUrl=environment.apiUrl
  //connecto message hub
   hubUrl=environment.hubUrl;
   private hubConnection?: HubConnection ;
   private messageThreadSource = new BehaviorSubject<string[]>([]) ; 
   messageThread$=this.messageThreadSource.asObservable() ;

  constructor(private http:HttpClient) { }

  createHubConnection(user:User , otherUsername:string){
    this.hubConnection= new HubConnectionBuilder()
    .withUrl(this.hubUrl+'message?user='+ otherUsername,
    //to pass the usertoken
    {accessTokenFactory:()=>user.token})
    //client if losse connection wil reconect
    .withAutomaticReconnect()
    .build() ; 
    this.hubConnection.start().catch(error =>console.log(error));

    this.hubConnection.on('ReceiveMessageThread',messages => {
      this.messageThreadSource.next(messages) ;
    })


    this.hubConnection.on('NewMessage', message=>{
      this.messageThread$.pipe(take(1)).subscribe({
        next:messages=>{
          this.messageThreadSource.next([...messages,message])
        }
      })
    })

  
  }

  stopHubConnection(){
    if(this.hubConnection){
      this.hubConnection.stop() ;
    }
   
  }
  



  getMessages(pageNumber:number ,pageSize:number , container: string){

    let params=getPaginationHeaders(pageNumber,pageSize) ;
    params=params.append('Container',container) ; 
    return getPaginatedResult<Message[]>(this.baseUrl+'messages',params,this.http) ;

  }


  getMessageThread(username:string){
    return this.http.get<Message[]>(this.baseUrl+'messages/thread/'+username)
  }


  sendMessage(RecipientUsername:string , Content:string) {

   // return this.http.post<Message>(this.baseUrl +'messages',{RecipientUsername,Content})
   return this.hubConnection?.invoke('sendMessage',{RecipientUsername, Content})
   .catch(error =>console.log("eni lena " +error));
   
  }

  deleteMessage(id:number) { 

    return this.http.delete(this.baseUrl+'messages/'+id) ;

  }


}
