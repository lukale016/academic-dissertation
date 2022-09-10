import { DateTime } from 'luxon';
import { RegisterAppointmentComponent } from './registerAppointment/register-appointment/register-appointment.component';
import { MatDialog } from '@angular/material/dialog';
import { AppointmentService } from './../Services/appointment/appointment.service';
import { Appointment } from './../models/Appointment';
import { AppointmentCriteria } from './../../criterias/appointmentCriteria';
import { environment } from './../../environments/environment';
import { MessagingHubService } from './../hubs/messaging/messaging.hub.service';
import { UserContainerService } from './../container/userContainer/user-container.service';
import { UserService } from './../Services/user/user.service';
import { BehaviorSubject, Subject, takeUntil, concatMap, first } from 'rxjs';
import { SuperUser } from 'src/app/models/SuperUser';
import { Component, OnInit, OnDestroy, ChangeDetectorRef, ViewChild } from '@angular/core';
import { MatDrawer } from '@angular/material/sidenav';

@Component({
  selector: 'app-appointments',
  templateUrl: './appointments.component.html',
  styleUrls: ['./appointments.component.css']
})
export class AppointmentsComponent implements OnInit,OnDestroy {
  public user?: SuperUser;
  public appName: string = environment.appName;
  public showBadge = false;
  public criteriaBind: AppointmentCriteria = new AppointmentCriteria();
  public allAppointments: Map<string, Appointment[]> = new Map<string, Appointment[]>();
  public appointmentsByDates: Map<string, Appointment[]> = new Map<string, Appointment[]>();
  private criteria: BehaviorSubject<AppointmentCriteria> = new BehaviorSubject<AppointmentCriteria>(this.criteriaBind);
  private $criteria = this.criteria.asObservable();

  @ViewChild("chatContainer") chatContainer?: MatDrawer;
  private $destroy: Subject<void> = new Subject<void>()
  
  constructor(private userService: UserService,
    private userContainer: UserContainerService,
    private changeDetector: ChangeDetectorRef,
    private messageHub: MessagingHubService,
    private appointmentService: AppointmentService,
    private dialog: MatDialog) { }


  ngOnInit(): void {
    this.userContainer.$user.pipe(takeUntil(this.$destroy))
      .subscribe((user: SuperUser | undefined) => {
        if(!user)
          this.userService.loadUserByToken();
        this.user = user;
        this.changeDetector.detectChanges();
      });
    
    this.appointmentService.getAppointments().pipe(takeUntil(this.$destroy))
      .subscribe((allAppointments: Map<string, Appointment[]>) => {
        this.allAppointments = allAppointments;
      })
    
    this.$criteria.pipe(takeUntil(this.$destroy),
    concatMap((appointmentCriteria: AppointmentCriteria) => this.appointmentService.search(appointmentCriteria)))
    .subscribe((appointmentsByDates: Map<string, Appointment[]>) => {
      this.appointmentsByDates = appointmentsByDates;
    });
  }

  messageArrived() {
    this.showBadge = true;
  }

  logout() {
    this.userService.logout();
  }

  async acceptRequest(patient: string) {
    await this.messageHub.acceptRequest(patient);
  }
  
  async rejectRequest(patient: string) {
    await this.messageHub.rejectRequest(patient);
  }

  applyFilters() {
    this.criteria.next(this.criteriaBind);
  }

  clearFilters() {
    this.criteriaBind = new AppointmentCriteria();
    this.criteria.next(this.criteriaBind);
  }

  deleteAppointment(appointment: Appointment) {
    this.appointmentService.deleteAppointment(appointment.id!).pipe(takeUntil(this.$destroy))
      .subscribe((data: string) => {
        let id: number = Number.parseInt(data);
        if(id) {
            this.criteria.next(this.criteriaBind);
        }
      })
  }

  openRegisterAppointmentDialog() {
    if(!this.user)
      return;
    const dialogRef = this.dialog.open(RegisterAppointmentComponent, {
      width: "800px",
      data: this.user
    });

    dialogRef.afterClosed().pipe(first())
      .subscribe(() => {
        this.criteriaBind = new AppointmentCriteria();
        this.criteria.next(this.criteriaBind);
      });
  }

  getTimeFromIsoDateString(date: string) {
    const time: DateTime = DateTime.fromISO(date);
    return time.toFormat("HH:mm");
  }

  ngOnDestroy(): void {
    this.$destroy.next();
    this.$destroy.complete();
  }
}
