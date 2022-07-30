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
  appName: string = environment.appName;
  user?: SuperUser;
  doctors: Doctor[] = [];
  criteriaBind: DoctorCriteria = new DoctorCriteria();
  specializations: string[] = [];
  private criteria: BehaviorSubject<DoctorCriteria> = new BehaviorSubject<DoctorCriteria>(this.criteriaBind);
  private $criteria = this.criteria.asObservable();

  private $destroy: Subject<void> = new Subject<void>();
  
  @ViewChild("chatContainer") chatContainer?: MatDrawer;

  constructor(private userService: UserService,
    private changeDetector: ChangeDetectorRef) { }
  
  ngOnInit(): void {
    this.userService.$user.pipe(takeUntil(this.$destroy))
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

  private onlyUnique(value: any, index: number, self: any) {
    return self.indexOf(value) === index;
  }

  ngOnDestroy(): void {
    this.$destroy.next();
    this.$destroy.complete();
  }
}
