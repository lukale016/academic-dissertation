import { Subject, takeUntil } from 'rxjs';
import { SuperUser } from './../../models/SuperUser';
import { UserService } from './../../Services/user/user.service';
import { Component, OnInit, OnDestroy } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit, OnDestroy {
  public user: SuperUser | undefined;
  public isDoctor: boolean = false; 
  private $destroy: Subject<void> = new Subject<void>();

  constructor(private userService: UserService) { }

  ngOnInit(): void {
    this.userService.$isDoctor.pipe(takeUntil(this.$destroy))
                    .subscribe((isDoctor: boolean) => this.isDoctor = isDoctor);
    this.userService.$user.pipe(takeUntil(this.$destroy))
                    .subscribe((user: SuperUser) => {
                      this.user = user;
                      console.log(user);
                    });
  }

  ngOnDestroy(): void {
    this.$destroy.next();
    this.$destroy.complete();
  }

}
