import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { take } from 'rxjs';
import { User } from '../_models/user';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
})
export class HomeComponent implements OnInit {
  user: User;
  registerMode = false;
  users: any;
  constructor(
    private http: HttpClient,
    private accountService: AccountService
  ) {}
  ngOnInit(): void {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: (user) => {
        if (user) this.user = user;
      },
    });
  }
  registerToggle() {
    this.registerMode = !this.registerMode;
  }

  getUsers() {
    this.http.get('https://localhost:5001/api/users').subscribe({
      next: (Response) => (this.users = Response),
      error: (error) => console.log(error),
      complete: () => console.log('req completed'),
    });
  }

  onCancel(event: boolean) {
    this.registerMode = event;
  }
}
