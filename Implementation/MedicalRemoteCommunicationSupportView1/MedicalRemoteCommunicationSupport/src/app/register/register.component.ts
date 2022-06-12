import { PatientPostDto } from './../Dtos/PatientPostDto';
import { UserService } from './../Services/user/user.service';
import { NavigationEnd, NavigationStart, Router } from '@angular/router';
import { ModelValidator } from './../helpers/ModelValidator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { DoctroPostDto } from './../Dtos/DoctorPostDto';
import { environment } from './../../environments/environment';
import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { filter, takeUntil, Subject } from 'rxjs';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit, OnDestroy {

  public data: DoctroPostDto = new DoctroPostDto();
  public appName: string = environment.appName;
  public isDoctor = false;
  private $destroy: Subject<void> = new Subject<void>();

  constructor(private router: Router, private snack: MatSnackBar, private userService: UserService) { 
    this.router.events
      .pipe(
        filter(e => e instanceof NavigationEnd),
        takeUntil(this.$destroy))
      .subscribe((e) => {
        const navigation  = this.router.getCurrentNavigation();
        this.isDoctor = navigation?.extras.state ? navigation?.extras.state['isDoc'] : false;
      });
  }

  ngOnInit(): void {
  }

  register() {
    let modelFilled: boolean = this.isDoctor ? ModelValidator.validate(this.data) : ModelValidator.validate(this.data, ["specialization"]);
    if(!modelFilled) {
      this.snack.open("Fill all required fields", "close", { duration: 2000});
      return;
    }
    if(this.isDoctor)
      this.userService.addUser(this.data);
    else
      this.userService.addUser(new PatientPostDto(this.data));
  }

  ngOnDestroy(): void {
    this.$destroy.next();
    this.$destroy.complete();
  }
}
