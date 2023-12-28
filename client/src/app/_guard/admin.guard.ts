import { CanActivateFn } from '@angular/router';
import { AccountService } from '../_services/account.service';
import { inject } from '@angular/core';
import { map } from 'rxjs';
import { ToastrService } from 'ngx-toastr';

export const adminGuard: CanActivateFn = (route, state) => {
  const accountservice=inject(AccountService) ;
  const toaster=inject(ToastrService) ;
  
  return accountservice.currentUser$.pipe(
    map(user=>{

      if(!user) return false
 
      if(user.roles.includes('Admin') || user.roles.includes('Moderator')) {
      
      return true ;
      }else 
      toaster.error('you not allow to get this resources') ;
      return false;
      

    }
  )
  )
};
