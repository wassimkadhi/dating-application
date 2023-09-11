import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Member } from 'src/app/_models/member';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent  implements OnInit{

  /**
   *
   */
  constructor( private router :Router) {
    
    
  }

  @Input() member :Member 
  ngOnInit(): void {
    
  }

 
}
