import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Toast, ToastrService } from 'ngx-toastr';
import { take } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css'],
})
export class MemberEditComponent implements OnInit {
  member: Member;
  user: User;
  @HostListener('window:beforeunload' , [`$event`]) unloadNonotification($event:any){
    $event.returnValue=true ;
  }
  @ViewChild(`editForm`) editForm: NgForm;

  /**
   *
   */
  constructor(
    private memberService: MembersService,
    private accountService: AccountService,
    private toaster: ToastrService
  ) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: (user) => (this.user = user),
    });
  }
  ngOnInit(): void {
    this.loadMember();
  }

  loadMember() {
    if (!this.user) return;
    this.memberService.getMemeber(this.user.username).subscribe({
      next: (member) => (this.member = member),
    });
  }

  updateMember() {
    
    this.memberService.editMmember(this.editForm.value).subscribe({
      next: (_) => {
        this.toaster.success('profile updatetd succefully');
        this.editForm.reset(this.member);
      },
    });
  }
}
