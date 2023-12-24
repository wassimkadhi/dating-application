import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { map, of, take } from 'rxjs';
import { PaginatedResult } from '../_models/pagination';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';
import { User } from '../_models/user';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';


@Injectable({
  providedIn: 'root',
})
export class MembersService {
  baseUrl = environment.apiUrl;
  members :Member[] = [] ; 
  memberCache=new Map() ;
  user:User|undefined ; 
  userParams:UserParams|undefined
  

 
  constructor(private http: HttpClient,private accountService :AccountService) {

    this.accountService.currentUser$.pipe(take(1)).subscribe({

      next:user=>{
        if(user) {
           this.userParams=new UserParams(user) ;
          this.user=user

        }
        
      }
     })
  }




getUserParams(){
  return this.userParams ; 
}

setUserParams(params:UserParams) {
  this.userParams=params ; 
}

resetUserParams(){
  if(this.user) {
    this.userParams=new UserParams(this.user);
    return this.userParams;
  }
    
}
  getMembers(userParams :UserParams) {
    const response =this.memberCache.get(Object.values(userParams).join('-')) ;
    if(response) return of(response);
  

    let params = getPaginationHeaders(userParams.pageNumber ,userParams.pageSize);
    params=params.append('minAge', userParams.minAge) ; 
    params=params.append('maxAge', userParams.maxAge) ; 
    params=params.append('gender', userParams.gender) ;
    params=params.append('orderBy',userParams.orderBy);

    return getPaginatedResult<Member[]>( this.baseUrl + 'users', params ,this.http).pipe(
      map(response=>{
        this.memberCache.set(Object.values(userParams).join('-'),response) ; 
        return response ; 
      })
    ) 
  }



  
   getMemeber(username:string ) {
    const member=[...this.memberCache.values()]
    .reduce((arr,elem)=>arr.concat(elem.result) , [])
    .find((m:Member)=>m.userName===username); 

    if(member) return of(member) ; 
    
    
    return this.http.get<Member>(this.baseUrl +'users/' + username ) ;

   }

   editMmember(member:Member) {
    
    return this.http.put(this.baseUrl + 'users/EditMember', member) ;
    
   }


   editMainPhoto(photoId :number ) {

    return this.http.put(this.baseUrl+'users/edit-main-photo/'+ photoId ,{} ) ; 

   }


   deletePhotoById(photoId : number) {

    return this.http.delete(this.baseUrl+'users/delete-photo/' + photoId) ; 
   }
   

   


 likeMember(username :string)
 {
    return this.http.post(this.baseUrl+'likes/'+ username,{})  ; 


 }

 getLikes(predicate:string) {
  return this.http.get<Member[]>(this.baseUrl+'likes?predicate=' + predicate) ;  
 }
 
}
