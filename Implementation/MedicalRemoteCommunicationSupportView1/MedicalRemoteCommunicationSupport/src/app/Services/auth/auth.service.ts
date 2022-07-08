import { tokenKey } from './../../constants/localStorageConsts';
import { MatSnackBar } from '@angular/material/snack-bar';
import { UserAndToken } from './../../Dtos/UserAndToken';
import { mergeMap, of, EMPTY } from 'rxjs';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { LoginCreds } from 'src/app/models/LoginCreds';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private loggedIn: boolean = false;
  private rootRoute: string = environment.serverRoute + "auth/";

  constructor(private client: HttpClient, private snack: MatSnackBar) { }

  login(creds: LoginCreds) : any {
    let headers = new HttpHeaders();
    headers = headers.append("ContentType", "application/json")
                     .append("Allow", "*");
    return this.client.post<UserAndToken>(`${this.rootRoute}login`, creds, {
      headers: headers
    }).pipe(
      mergeMap((data: UserAndToken) => {
        if(!data.user || !data.token || data.token === "")
        {
          this.snack.open("Something went wrong please try again", "close", { duration: 2000});
          return EMPTY;
        }
        localStorage.setItem(tokenKey, data.token);
        return of(data.user);
      })
    );
  }

  setLoggedStatus(status: boolean)
  {
    this.loggedIn = status;
  }

  isLoggedIn(): boolean
  {
    return this.loggedIn;
  }
}
