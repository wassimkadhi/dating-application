import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Route, Router } from '@angular/router';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TabDirective, TabsModule, TabsetComponent } from 'ngx-bootstrap/tabs';
import { TimeagoModule } from 'ngx-timeago';
import { Member } from 'src/app/_models/member';
import { MemberMessagesComponent } from '../member-messages/member-messages.component';
import { MessageService } from 'src/app/_services/message.service';
import { Message } from 'src/app/_models/message';
import { PresenceService } from 'src/app/_services/presence.service';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { take } from 'rxjs';

@Component({
  selector: 'app-member-detail',
  standalone: true,
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css'],
  imports: [
    CommonModule,
    TabsModule,
    GalleryModule,
    TimeagoModule,
    MemberMessagesComponent,
  ],
})
export class MemberDetailComponent implements OnInit, OnDestroy{
  @ViewChild('memberTabs',{static:true}) memberTabs?: TabsetComponent;
  username: string;
  member: Member ={} as Member;
  images: GalleryItem[] = [];
  activeTab?: TabDirective;
  messages: Message[] = [];
  user?:User ; 

  constructor(
    private route: ActivatedRoute,
    private messageService: MessageService,
    public presenceService :PresenceService ,
    private accountservice :AccountService 
  ) {

    this.accountservice.currentUser$.pipe(take(1)).subscribe({

      next:user=>{
        if(user) this.user=user;
      }
  
    })
    

  }
  ngOnDestroy(): void {

    this.messageService.stopHubConnection() ;
  }
  ngOnInit(): void {
    this.route.data.subscribe({
      next:data=>{this.member=data['member'];
     
      this.username=this.member.userName;}
    })

    this.route.queryParams.subscribe({
      next: (params) => {
        params['Tab'] && this.selectTab(params['Tab']);
      },
    });

    this.getImages();
  }

  selectTab(heading: string) {
    this.memberTabs.tabs.find((x) => x.heading === heading)!.active = true;
  }

  onTabActivated(data: TabDirective) {
    this.activeTab = data;
    if (this.activeTab.heading === 'Messages' && this.user) {
           this.messageService.createHubConnection(this.user ,this.member.userName) ;

      //this.loadMessages();
    }
    else{
      this.messageService.stopHubConnection() ;
    }
  }

  loadMessages() {
    if (this.username) {
      this.messageService.getMessageThread(this.username).subscribe({
        next: (response) => (this.messages = response),
      });
    }
  }
  getImages() {
    for (const photo of this.member.photos) {
      this.images.push(new ImageItem({ src: photo.url, thumb: photo.url }));
    }
  }

}
