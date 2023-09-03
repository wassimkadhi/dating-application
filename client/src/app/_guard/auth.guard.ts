import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { map } from 'rxjs';
// to protects roots 
export const authGuard: CanActivateFn = (route, state) => {
  // beacause its not a class w inject the services using inject 
  const accountservice=inject(AccountService) ; 
  const toaster=inject(ToastrService) ;

  //verifiey if we have user connected to the app 
 return(accountservice.currentUser$.pipe(
  // because its an absorvalbe we use .pipe 
  map(user=>{
    if(user)  return true;
    else{
      toaster.error('you need to logIn first ') ;
      return false ;
    }
  })
 ))
  
};
