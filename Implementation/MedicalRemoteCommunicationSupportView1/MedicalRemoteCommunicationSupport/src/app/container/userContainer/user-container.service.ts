import { BehaviorSubject, Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { SuperUser } from 'src/app/models/SuperUser';

@Injectable({
  providedIn: 'root'
})
export class UserContainerService {
  user: BehaviorSubject<SuperUser | undefined> = new BehaviorSubject<SuperUser | undefined>(undefined);
  $user: Observable<SuperUser | undefined> = this.user.asObservable();

  constructor() { }
}
