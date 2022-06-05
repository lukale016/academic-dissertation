/// <reference path="../../models/Models.ts" />
import { AuthService } from './../auth/auth.service';
import { HttpClient } from '@angular/common/http';
import { destroyPlatform, Injectable, OnDestroy, Type } from '@angular/core';
import { Observable, Subject, takeUntil } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserService implements OnDestroy {
  private user: Subject<models.Doctor> | Subject<models.Patient>;
  public $user: Observable<models.Doctor> | Observable<models.Patient>;
  private $destroy: Subject<void> = new Subject<void>();
  private docType: Type<models.Doctor>;

  constructor(private client: HttpClient, private authService: AuthService) { }
  
  ngOnDestroy(): void {
    this.$destroy.next();
    this.$destroy.complete();
  }

  loadUser(creds: models.LoginCreds)
  {
    
    this.authService.logIn(creds).pipe(takeUntil(this.$destroy)).subscribe(data => {
      if(data instanceof models.Doctor)
    });
  }

}
