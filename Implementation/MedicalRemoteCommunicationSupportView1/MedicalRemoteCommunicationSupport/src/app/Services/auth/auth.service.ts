import { environment } from '../../../environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { LoginCreds } from 'src/app/models/LoginCreds';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private loggedIn: boolean = false;
  private routeAppend: string = "auth/";

  constructor(private client: HttpClient ) { }

  logIn(creds: LoginCreds)
  {
    let headers = new HttpHeaders();
    headers.append("ContentType", "application/json");
    headers.append("Allow", "*");
    return this.client.post(`${environment.serverRoute}${this.routeAppend}`, creds, {
      headers: headers
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
}
