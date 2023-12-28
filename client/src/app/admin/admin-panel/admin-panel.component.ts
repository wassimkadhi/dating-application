import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/_models/user';
import { AdminService } from 'src/app/_services/admin.service';

@Component({
  selector: 'app-admin-panel',
  templateUrl: './admin-panel.component.html',
  styleUrls: ['./admin-panel.component.css']
})
export class AdminPanelComponent implements OnInit {

  users : User[]
  
  constructor( private adminservices :AdminService) {}
  ngOnInit(): void {
    this.adminservices.getUsersRoles().subscribe({

      next:re=>this.users=re 
     }
     
     )
  }
 
 
}
