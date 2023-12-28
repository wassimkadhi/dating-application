import { Directive, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';
import { take } from 'rxjs';

@Directive({
  selector: '[appHasRole]'
})
export class HasRoleDirective  implements OnInit{
  @Input()appHasRole: string[] =[]; 
  user : User ={} as User 
 
  constructor(private viewContainerRef :ViewContainerRef, private templateRef:TemplateRef<any>,private accountservice:AccountService) { 


    this.accountservice.currentUser$.pipe(take(1)).subscribe({
      next:user=>{
        if(user) this.user=user

      }
    })
  }

  ngOnInit(): void {
    if (this.user.roles.some(s=>this.appHasRole.includes(s)))
    {
      this.viewContainerRef.createEmbeddedView(this.templateRef)
    }else {
      this.viewContainerRef.clear() 
    }
  }

}
