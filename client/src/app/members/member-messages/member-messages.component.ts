import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, Input, OnInit, afterNextRender } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TimeagoModule } from 'ngx-timeago';
import { Message } from 'src/app/_models/message';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  changeDetection:ChangeDetectionStrategy.OnPush,
  selector: 'app-member-messages',
  standalone: true,
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css'],
  imports: [CommonModule, TimeagoModule ,FormsModule],
})
export class MemberMessagesComponent implements OnInit {
  // avoir username from parent component
  @Input() username?: string;
  messagecontent: string;
  
  constructor(public messageService: MessageService) {}
  ngOnInit(): void {
   
  }

  
  onSubmit() {

    
    this.messageService.sendMessage(this.username,this.messagecontent).then(()=>{
      this.messagecontent=null;
      console.log(this.username) ;
    })
  }
}
