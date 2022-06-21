import { Subject, takeUntil } from 'rxjs';
import { UserService } from './../../Services/user/user.service';
import { environment } from './../../../environments/environment';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { LoginCreds } from 'src/app/models/LoginCreds';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit, OnDestroy {

  public appName: string = environment.appName
  public username: string = '';
  public password: string = '';
  private $destroy: Subject<void> = new Subject<void>();

  constructor(private userService: UserService, private router: Router) { }

  ngOnInit(): void {
  }

  login() {
    this.userService.loadUser(new LoginCreds(this.username, this.password));
  }

  ngOnDestroy(): void {
    this.$destroy.next();
    this.$destroy.complete();
  }
}
