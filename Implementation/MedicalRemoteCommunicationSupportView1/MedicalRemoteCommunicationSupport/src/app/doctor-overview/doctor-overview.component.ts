import { UserContainerService } from './../container/userContainer/user-container.service';
import { MessagingHubService } from './../hubs/messaging/messaging.hub.service';
import { Router } from '@angular/router';
import { Doctor } from 'src/app/models/Doctor';
import { environment } from './../../environments/environment';
import { first, Subject, takeUntil, BehaviorSubject, concatMap } from 'rxjs';
import { UserService } from './../Services/user/user.service';
import { SuperUser } from './../models/SuperUser';
import { Component, OnInit, OnDestroy, ViewChild, ChangeDetectorRef } from '@angular/core';
import { MatDrawer } from '@angular/material/sidenav';
import { DoctorCriteria } from 'src/criterias/doctorCriteria';

@Component({
  selector: 'app-doctor-overview',
  templateUrl: './doctor-overview.component.html',
  styleUrls: ['./doctor-overview.component.css']
})
export class DoctorOverviewComponent implements OnInit, OnDestroy {
  public appName: string = environment.appName;
  public user?: SuperUser;
  public doctors: Doctor[] = [];
  public criteriaBind: DoctorCriteria = new DoctorCriteria();
  public specializations: string[] = [];
  public showBadge: boolean = false;
  private criteria: BehaviorSubject<DoctorCriteria> = new BehaviorSubject<DoctorCriteria>(this.criteriaBind);
  private $criteria = this.criteria.asObservable();

  private $destroy: Subject<void> = new Subject<void>();
  
  @ViewChild("chatContainer") chatContainer?: MatDrawer;

  constructor(private userService: UserService,
    private userContainer: UserContainerService,
    private changeDetector: ChangeDetectorRef,
    private messageHub: MessagingHubService) { }
  
  ngOnInit(): void {
    this.userContainer.$user.pipe(takeUntil(this.$destroy))
      .subscribe((user: SuperUser | undefined) => {
        if(!user)
          this.userService.loadUserByToken();
        this.user = user;
        this.changeDetector.detectChanges();
      });
    
    this.$criteria.pipe(takeUntil(this.$destroy),
    concatMap((criteria: DoctorCriteria) => this.userService.searchDoctors(criteria)))
    .subscribe((doctors: Doctor[]) => {
      this.doctors = doctors;
      if(this.specializations.length == 0)
        this.specializations = doctors.map(doc => doc.specialization).filter(this.onlyUnique);
      this.changeDetector.detectChanges();
    });
  }

  applyFilters() {
    this.criteria.next(this.criteriaBind);
  }

  clearFilters() {
    this.criteriaBind = new DoctorCriteria();
    this.criteria.next(this.criteriaBind);
  }

  logout() {
    this.userService.logout();
  }

  async sendRequest(doctor: string) {
    await this.messageHub.sendRequest(doctor);
  }

  alreadySentOrAccepted(doctor: string, specialization: string): boolean {
    let sent = this.user?.sentRequests.filter(req => req.username == doctor || req.specialization == specialization);
    let accepted = this.user?.myDoctors.filter(doc => doc.username == doctor);
    let myDoctors = this.doctors.filter(doc => this.user?.myDoctors.map(myDoc => myDoc.username).includes(doc.username));
    return sent?.length != 0 || accepted?.length != 0 || myDoctors.map(myDoc => myDoc.specialization).includes(specialization);
  }

  private onlyUnique(value: any, index: number, self: any) {
    return self.indexOf(value) === index;
  }

  messageArrived() {
    this.showBadge = true;
  }

  async acceptRequest(patient: string) {
    await this.messageHub.acceptRequest(patient);
  }
  
  async rejectRequest(patient: string) {
    await this.messageHub.rejectRequest(patient);
  }

  openChats() {
    this.chatContainer?.toggle();
    this.showBadge = false;
  }

  ngOnDestroy(): void {
    this.$destroy.next();
    this.$destroy.complete();
  }
}
