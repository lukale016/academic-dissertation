import { UserContainerService } from './../../container/userContainer/user-container.service';
import { PatientCriteria } from './../../../criterias/patientCriteria';
import { DoctorCriteria } from './../../../criterias/doctorCriteria';
import { MessagingHubService } from './../../hubs/messaging/messaging.hub.service';
import { tokenKey } from './../../constants/localStorageConsts';
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
  private isDoctor: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  public $isDoctor: Observable<boolean> = this.isDoctor.asObservable();
  private $destroy: Subject<void> = new Subject<void>();

  private rootRoute: string = environment.serverRoute + "user/"

  constructor(private client: HttpClient, 
    private router: Router,
    private userContainer: UserContainerService,
    private authService: AuthService, 
    private messagingHub: MessagingHubService,
    private snack: MatSnackBar) { }
  
  ngOnDestroy(): void {
    this.$destroy.next();
    this.$destroy.complete();
  }

  loadUser(creds: LoginCreds)
  {
    this.authService.login(creds).pipe(takeUntil(this.$destroy))
      .subscribe({
      next: async (data : any) => {
        if(data.isDoctor == undefined)
        {
          this.userContainer.user.next(undefined);
          await this.messagingHub.Disconnect();
        }
        else if(data.isDoctor) {
          this.userContainer.user.next(new SuperUser(data as Doctor));
          this.isDoctor.next(true);
          await this.messagingHub.Connect();
        }
        else {
          this.userContainer.user.next(new SuperUser(data as Patient));
          this.isDoctor.next(false);
          await this.messagingHub.Connect();
        }
        this.router.navigate([""]);
      },
      error: (error: HttpErrorResponse) => {
        this.snack.open(error.error, "close", { duration: 2000 })
      }
    });
  }

  loadUserByToken() {
    this.authService.loginUserWithToken().pipe(takeUntil(this.$destroy))
      .subscribe({
        next: async (data) => {
          if(data.isDoctor == undefined)
          {
            this.userContainer.user.next(undefined);
            await this.messagingHub.Disconnect();
          }
          else if(data.isDoctor) {
            this.userContainer.user.next(new SuperUser(data as Doctor));
            this.isDoctor.next(true);
            await this.messagingHub.Connect();
          }
          else {
            this.userContainer.user.next(new SuperUser(data as Patient));
            this.isDoctor.next(false);
            await this.messagingHub.Connect();
          }
        },
        error: async (error: HttpErrorResponse) => {
          if(error.status === 401)
          {
            localStorage.removeItem(tokenKey);
            await this.messagingHub.Disconnect();
            return;
          }
        }
      });
  }

  addUser(data: DoctroPostDto | PatientPostDto) {
    if(data instanceof PatientPostDto) {
      this.client.post(`${this.rootRoute}AddPatient`, data as PatientPostDto, { 
        headers: this.getDefaultHeaders(),
        responseType: "text" 
      })
        .pipe(takeUntil(this.$destroy))
        .subscribe({
          next: data => {
            console.log(data) 
            this.snack.open(data, "close", { duration: 2000 }); 
            this.router.navigate(['login']);
          }, 
          error: (error: HttpErrorResponse) => { 
            this.snack.open(error.error, "close", { duration: 2000 });
          }
      });
    }
    else {
      this.client.post(`${this.rootRoute}AddDoctor`, data as DoctroPostDto, { 
        headers: this.getDefaultHeaders(),
        responseType: "text"
      })
        .pipe(takeUntil(this.$destroy))
        .subscribe({
          next: data => { 
            console.log(data)
            this.snack.open(data, "close", { duration: 2000 });
            this.router.navigate(['login']);
          }, 
          error: (error: HttpErrorResponse) => { 
            this.snack.open(error.error, "close", { duration: 2000 }); 
          }
      });
    }
  }

  searchDoctors(criteria: DoctorCriteria): Observable<Doctor[]> {
    return this.client.post<Doctor[]>(`${this.rootRoute}SearchDoctors`, criteria, { headers: this.getDefaultHeaders() });
  }

  searchPatients(criteria: PatientCriteria): Observable<Patient[]> {
    return this.client.post<Patient[]>(`${this.rootRoute}SearchPatients`, criteria, { headers: this.getDefaultHeaders() });
  }

  async logout() {
    localStorage.removeItem(tokenKey);
    this.userContainer.user.next(undefined);
    await this.messagingHub.Disconnect();
    this.router.navigate([""]);
  }

  private getDefaultHeaders() : HttpHeaders {
    let headers = new HttpHeaders();
    headers = headers.append("ContentType", "application/json")
                     .append("Allow", "*");
    const token: string | null = localStorage.getItem(tokenKey);
    if(token)
      headers = headers.append("Authorization", `Bearer ${token}`)
    return headers;
  }
}
