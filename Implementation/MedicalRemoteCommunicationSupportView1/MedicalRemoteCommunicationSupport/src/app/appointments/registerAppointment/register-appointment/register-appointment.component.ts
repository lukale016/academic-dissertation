import { HttpErrorResponse } from '@angular/common/http';
import { Appointment } from './../../../models/Appointment';
import { AppointmentPostDto } from './../../../Dtos/AppointmentPostDto';
import { DateTime } from 'luxon';
import { AppointmentsComponent } from './../../appointments.component';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Subject, first } from 'rxjs';
import { AppointmentService } from './../../../Services/appointment/appointment.service';
import { SuperUser } from './../../../models/SuperUser';
import { Component, Inject, OnInit, OnDestroy } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-register-appointment',
  templateUrl: './register-appointment.component.html',
  styleUrls: ['./register-appointment.component.css']
})
export class RegisterAppointmentComponent implements OnInit, OnDestroy {
  public patientSelected: boolean = false;
  public dateSelected: boolean = false;
  public user!: SuperUser;
  public _date?: Date;
  public timeObj?: DateTime;
  public occupiedTimeSlots?: string[];
  private patient?: string;

  get date() {
    return this._date!;
  }

  set date(value: Date) {
    this._date = value;
    this.luxonDate = DateTime.fromJSDate(value);
  }

  private luxonDate?: DateTime;
  public _time?: string;

  get time() {
    return this._time!;
  }

  set time(value: string) {
    this._time = value;
    this.timeObj = DateTime.fromFormat(value, "hh:mm");
  }

  public duration?: number;
  public occupiedTimes?: string[];
  
  private $destory: Subject<void> = new Subject<void>();

  constructor(private dialogRef: MatDialogRef<AppointmentsComponent>,
    private appointmentServce: AppointmentService,
    private snack: MatSnackBar,
    @Inject(MAT_DIALOG_DATA) public data: SuperUser) { 
    this.user = data;
  }
  
  ngOnInit(): void {
  }

  dateChanged() {
    this.dateSelected = true;
    if(this.luxonDate)
      this.appointmentServce.getOccupiedTimeSlots(this.luxonDate.toFormat("M/d/yyyy")).pipe(first())
        .subscribe((occupied: string[]) => this.occupiedTimeSlots = occupied);
  }

  registerAppointment() {
    if(!this.time || !this.duration || this.duration === 0) {
      this.snack.open("Time and duration must be set", "close", { duration: 2000 });
      return;
    }
    this.luxonDate = this.luxonDate?.set({ 
      hour: this.timeObj?.hour,
      minute: this.timeObj?.minute
    });
    if(!this.user.username || !this.patient || !this.luxonDate) {
      this.snack.open("Fill all required data", "close", { duration: 2000 });
      return;
    }
    const dto = new AppointmentPostDto();
    dto.doctor = this.user.username;
    dto.patient = this.patient;
    dto.scheduledTime = this.luxonDate.toISO();
    dto.duration = this.duration!;
    this.appointmentServce.registerAppointment(dto).pipe(first())
        .subscribe({
          next: (appointment: Appointment) => {
            this.snack.open("Appointment added", "close", { duration: 2000 });
            this.dialogRef.close();
          },
          error: (error: HttpErrorResponse) => {
            this.snack.open(error.error, "close", { duration: 2000 });
          }
        })
  }

  patientChosen(selectedUsername: string) {
    this.patientSelected = true;
    this.patient = selectedUsername;
  }

  ngOnDestroy(): void {
    this.$destory.next();
    this.$destory.complete();
  }

  cancel() {
    this.dialogRef.close();
  }

  getMinDate(): Date {
    return new Date();
  }
}
