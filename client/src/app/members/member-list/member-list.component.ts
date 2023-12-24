import { Component, OnInit } from '@angular/core';
import { Observable, take } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { Pagination } from 'src/app/_models/pagination';
import { User } from 'src/app/_models/user';
import { UserParams } from 'src/app/_models/userParams';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css'],
})
export class MemberListComponent implements OnInit {
  // members$: Observable<Member[]>
  members: Member[] = [];
  pagination: Pagination| undefined;
  userParams: UserParams | undefined;

  genderList = [
    { value: 'male', display: 'Males' },
    { value: 'female', display: 'females' },
  ];

  constructor(private memberservice: MembersService) {
    this.userParams = this.memberservice.getUserParams();
  }
  ngOnInit(): void {
    this.loadMembers();

    // this.members$ = this.memberservice.getMembers();
  }

  loadMembers() {
    if (this.userParams) {
      this.memberservice.setUserParams(this.userParams);
      this.memberservice.getMembers(this.userParams).subscribe({
        next: (response) => {
          if (response.result && response.pagination) {
            this.members = response.result;
            this.pagination = response.pagination;
          }
        },
      });
    }
  }

  resetFilter() {
    this.userParams = this.memberservice.resetUserParams();
    this.loadMembers();
  }

  pagechanged(event: any) {
    if (this.userParams?.pageNumber != event.page) {
      this.userParams.pageNumber = event.page;
      this.memberservice.setUserParams(this.userParams);
      this.loadMembers();
    }
  }
}
