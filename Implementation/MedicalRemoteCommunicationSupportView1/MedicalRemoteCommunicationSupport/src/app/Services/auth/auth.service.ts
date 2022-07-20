import { tokenKey } from './../../constants/localStorageConsts';
import { MatSnackBar } from '@angular/material/snack-bar';
import { UserAndToken } from './../../Dtos/UserAndToken';
import { mergeMap, of, EMPTY, Observable } from 'rxjs';
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

  login(creds: LoginCreds) : Observable<any> {
    return this.client.post<UserAndToken>(`${this.rootRoute}login`, creds, {
      headers: this.defaultHeaders()
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

  loginUserWithToken(): Observable<any>{
    let token = localStorage.getItem(tokenKey);
    if(!token)
      return EMPTY;
    return this.client.get<any>(`${this.rootRoute}getUserFromToken`, {
      headers: this.defaultHeaders().append("Authorization", `Bearer ${token}`)
    })
  }

  setLoggedStatus(status: boolean)
  {
    this.loggedIn = status;
  }

  isLoggedIn(): boolean
  {
    return this.loggedIn;
  }

  private defaultHeaders(): HttpHeaders {
    let headers = new HttpHeaders();
    headers = headers.append("ContentType", "application/json")
                     .append("Allow", "*");
    return headers;
  }
}
