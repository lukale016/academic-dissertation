import { SuperUser } from './../../models/SuperUser';
import { Router } from '@angular/router';
import { PatientPostDto } from './../../Dtos/PatientPostDto';
import { DoctroPostDto } from './../../Dtos/DoctorPostDto';
import { environment } from './../../../environments/environment';
import { AuthService } from './../auth/auth.service';
import { HttpClient, HttpErrorResponse, HttpHeaders, HttpResponse } from '@angular/common/http';
import { Injectable, OnDestroy } from '@angular/core';
import { BehaviorSubject, Observable, Subject, takeUntil } from 'rxjs';
import { Doctor } from 'src/app/models/Doctor';
import { Patient } from 'src/app/models/Patient';
import { LoginCreds } from 'src/app/models/LoginCreds';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({
  providedIn: 'root'
})
export class UserService implements OnDestroy {
  private user: BehaviorSubject<SuperUser> = new BehaviorSubject<SuperUser>(new SuperUser(null));
  public $user: Observable<SuperUser> = this.user.asObservable();
  private isDoctor: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  public $isDoctor: Observable<boolean> = this.isDoctor.asObservable();
  private $destroy: Subject<void> = new Subject<void>();

  private rootRoute: string = environment.serverRoute + "user/"

  constructor(private client: HttpClient, private router: Router, private authService: AuthService, private snack: MatSnackBar) { }
  
  ngOnDestroy(): void {
    this.$destroy.next();
    this.$destroy.complete();
  }

  loadUser(creds: LoginCreds)
  {
    this.authService.login(creds).pipe(takeUntil(this.$destroy)).subscribe((data : any) => {
      if(data.isDoctor) {
        this.user.next(new SuperUser(data as Doctor));
        this.isDoctor.next(true);
      }
      else {
        this.user.next(new SuperUser(data as Patient));
        this.isDoctor.next(false);
      }
      this.router.navigate([""]);
    },
    (error: HttpErrorResponse) => {
      this.snack.open(error.error, "close", { duration: 2000 })
    });
  }

  addUser(data: DoctroPostDto | PatientPostDto) {
    if(data instanceof PatientPostDto) {
      this.client.post<string>(`${this.rootRoute}AddPatient`, data as PatientPostDto, { headers: this.getDefaultHeaders() })
        .pipe(takeUntil(this.$destroy))
        .subscribe(data => { 
          this.snack.open(data, "close", { duration: 2000 }); 
          this.router.navigate(['login']);
        }, 
          (error: HttpErrorResponse) => { 
            this.snack.open(error.error, "close", { duration: 2000 });
          }
        );
    }
    else {
      this.client.post<string>(`${this.rootRoute}AddDoctor`, data as DoctroPostDto, { headers: this.getDefaultHeaders()})
        .pipe(takeUntil(this.$destroy))
        .subscribe(data => { 
          this.snack.open(data, "close", { duration: 2000 });
          this.router.navigate(['login']);
        }, 
          (error: HttpErrorResponse) => { 
            this.snack.open(error.error, "close", { duration: 2000 }); 
          }
        );
    }
  }

  private getDefaultHeaders() : HttpHeaders {
    let headers = new HttpHeaders();
    headers.append("ContentType", "application/json");
    headers.append("Allow", "*");
    return headers;
  }
}
