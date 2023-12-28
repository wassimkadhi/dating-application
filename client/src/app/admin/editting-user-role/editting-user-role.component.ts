import { Component, Input, OnInit } from '@angular/core';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { User } from 'src/app/_models/user';
import { AdminService } from 'src/app/_services/admin.service';
import { RolesModalComponent } from 'src/app/modals/roles-modal/roles-modal.component';

@Component({
  selector: 'app-editting-user-role',
  templateUrl: './editting-user-role.component.html',
  styleUrls: ['./editting-user-role.component.css']
})
export class EdittingUserRoleComponent  implements OnInit{

  @Input() users:User ;
  bsModalRef:BsModalRef<RolesModalComponent> = new BsModalRef<RolesModalComponent>() ;
  availableRoles=[
    'Admin' , 
    'Moderator',
    'Member'
  ]

 
  constructor( private modalservice:BsModalService,private adminservice:AdminService ) {}

  ngOnInit(): void {
    
  }

  openModel(user:User) {
    const config={
      class:'model-dialog-center',
      initialState:{
        username:user.username , 
        availableroles:this.availableRoles,
        selectedroles: [...user.roles]
      }
    }
    this.bsModalRef=this.modalservice.show(RolesModalComponent,config) ; 
    this.bsModalRef.onHide?.subscribe(
      {
        next:()=>{
           const selectedroles =this.bsModalRef.content?.selectedroles; 
          if(!this.arrauEqual(selectedroles,user.roles)){
           
            this.adminservice.updateRoles(user.username , selectedroles!).subscribe({
              next:roles=>user.roles=roles
            })
          }


        }
      }
    )
    }
    
  private arrauEqual(arr1:any[] , arr2:any[]){

    return JSON.stringify(arr1.sort())===JSON.stringify(arr2.sort());
  }
  }


