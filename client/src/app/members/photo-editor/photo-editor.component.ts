import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FileUploader } from 'ng2-file-upload';
import { take } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { Photo } from 'src/app/_models/photo';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css'],
})
export class PhotoEditorComponent implements OnInit {
  @Input() member: Member;
  uploader: FileUploader;
  hasBaseDropZoneOver = false;
  user: User;
  baseUrl = environment.apiUrl;
  photom:Photo

  /**
   *
   */
  constructor(
    private accountService: AccountService,
    private router: Router,
    private memberService: MembersService
  ) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: (user) => {
        if (user) this.user = user;
      },
    });
  }

  ngOnInit(): void {
    this.initializeUpLoader();
  }

  fileOverBase(e: any) {
    this.hasBaseDropZoneOver = e;
  }

  initializeUpLoader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/add-photo',
      authToken: 'Bearer ' + this.user.token,
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024,
    });

    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false;
    };

    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        const photo = JSON.parse(response);
        this.member.photos.push(photo);
      }
    };
  }

  setmain(photo: Photo) {
    this.memberService.editMainPhoto(photo.id).subscribe({
      next: () => {
        if (this.user && this.member) {
          this.user.photoUrl = photo.url;
          this.accountService.setCurrentUser(this.user);
          this.member.urlPhoto = photo.url;
          this.member.photos.forEach((p) => {
            if (p.isMain) p.isMain = false;
            if (p.id == photo.id) p.isMain = true;
          });
        }
      },
    });
  }

  deletePhoto(photo:Photo) {
    this.memberService.deletePhotoById(photo.id).subscribe({
      next:()=>{
        if(photo.isMain)

        { 
          this.member.photos= this.member.photos.filter(x=>x.id != photo.id);
         this.photom=this.member.photos.find(x=>x.isMain==false);
        if(this.photom)
        { this.photom.isMain=true ; 
         this.user.photoUrl=this.photom.url ;
         this.accountService.setCurrentUser(this.user);
         this.member.urlPhoto = this.photom.url;
        }

         
        }

       this.member.photos= this.member.photos.filter(x=>x.id != photo.id)
       
      }
    })
  }
}
