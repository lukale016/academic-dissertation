import { AuthService } from './../auth/auth.service';
import { HttpClient } from '@angular/common/http';
import { Injectable, OnDestroy } from '@angular/core';
import { BehaviorSubject, Observable, Subject, takeUntil } from 'rxjs';
import { Doctor } from 'src/app/models/Doctor';
import { Patient } from 'src/app/models/Patient';
import { LoginCreds } from 'src/app/models/LoginCreds';

@Injectable({
  providedIn: 'root'
})
export class UserService implements OnDestroy {
  private doctor: BehaviorSubject<Doctor> = new BehaviorSubject<Doctor>(new Doctor());
  public $doctor: Observable<Doctor> = this.doctor.asObservable();
  private patient: BehaviorSubject<Patient> = new BehaviorSubject<Patient>(new Patient());
  public $patient: Observable<Patient> = this.patient.asObservable();
  private $destroy: Subject<void> = new Subject<void>();
  public isDoctor: boolean = false;

  constructor(private client: HttpClient, private authService: AuthService) { }
  
  ngOnDestroy(): void {
    this.$destroy.next();
    this.$destroy.complete();
  }

  loadUser(creds: LoginCreds)
  {
    this.authService.logIn(creds).pipe(takeUntil(this.$destroy)).subscribe(data => {
      if(data instanceof Doctor)
      {
        this.doctor.next(data as Doctor);
        this.isDoctor = true;
      }
      else
      {
        this.patient.next(data as Patient);
        this.isDoctor = false;
      }
    });
  }


}
