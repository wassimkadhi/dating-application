import { CommonModule } from '@angular/common';
import { Component, Input, OnInit, afterNextRender } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TimeagoModule } from 'ngx-timeago';
import { Message } from 'src/app/_models/message';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  selector: 'app-member-messages',
  standalone: true,
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css'],
  imports: [CommonModule, TimeagoModule ,FormsModule],
})
export class MemberMessagesComponent implements OnInit {
  // avoir username from parent component
  @Input() username?: string;
  @Input() messages: Message[] = [];
  messagecontent: string;
  
  constructor(private messageService: MessageService) {}
  ngOnInit(): void {
   
  }

  
  onSubmit() {

    console.log("fil fonction" + this.username+this.messagecontent)
    this.messageService.sendMessage(this.username,this.messagecontent).subscribe({
      next:message=>{this.messages.push(message);
       this.messagecontent=null;
}
    
    })

    
  }
}
