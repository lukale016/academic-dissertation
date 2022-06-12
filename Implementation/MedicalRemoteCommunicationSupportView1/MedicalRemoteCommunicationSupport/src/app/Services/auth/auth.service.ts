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

  constructor(private client: HttpClient ) { }

  login(creds: LoginCreds) : any {
    let headers = new HttpHeaders();
    headers.append("ContentType", "application/json");
    headers.append("Allow", "*");
    return this.client.post(`${this.rootRoute}login`, creds, {
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
