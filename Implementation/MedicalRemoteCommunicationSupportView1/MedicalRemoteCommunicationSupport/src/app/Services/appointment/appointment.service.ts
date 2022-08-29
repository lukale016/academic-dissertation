import { AppointmentCriteria } from './../../../criterias/appointmentCriteria';
import { AppointmentPostDto } from './../../Dtos/AppointmentPostDto';
import { Appointment } from './../../models/Appointment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from './../../../environments/environment';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { tokenKey } from 'src/app/constants/localStorageConsts';

@Injectable({
  providedIn: 'root'
})
export class AppointmentService {
  private rootRoute: string = environment.serverRoute + "appointment/";

  constructor(private client: HttpClient) { }

  getAppointments(): Observable<Map<string, Appointment[]>> {
    return this.client.get<Map<string, Appointment[]>>(`${this.rootRoute}getAppointments`, { headers: this.getDefaultHeaders() });
  }

  getOccupiedTimeSlots(date: string) {
    return this.client.post<string[]>(`${this.rootRoute}occupiedTimeSlots`, { date } ,{ headers: this.getDefaultHeaders() });
  }

  registerAppointment(appointment: AppointmentPostDto): Observable<Appointment> {
    return this.client.post<Appointment>(`${this.rootRoute}registerAppointment`, appointment,{ headers: this.getDefaultHeaders() });
  }

  deleteAppointment(id: number) {
    return this.client.delete(`${this.rootRoute}deleteAppointment/${id}`, { 
      headers: this.getDefaultHeaders(),
      responseType: "text"
    });
  }

  search(criteria: AppointmentCriteria): Observable<Map<string, Appointment[]>> {
    return this.client.post<Map<string, Appointment[]>>(`${this.rootRoute}search`, criteria,{ headers: this.getDefaultHeaders() });
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
