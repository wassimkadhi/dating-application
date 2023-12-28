import { Component, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-roles-modal',
  templateUrl: './roles-modal.component.html',
  styleUrls: ['./roles-modal.component.css']
})
export class RolesModalComponent implements OnInit {

username=''; 
availableroles:any[]=[] ;
selectedroles:any[]=[];
 
 constructor(public bsModalRef:BsModalRef , ) {
  
 }
 ngOnInit(): void {}

 updateCheked(checkedValue:string){

  const index=this.selectedroles.indexOf(checkedValue) ;
  index!==-1? this.selectedroles.splice(index,1):this.selectedroles.push(checkedValue);
 }
 
 
}
