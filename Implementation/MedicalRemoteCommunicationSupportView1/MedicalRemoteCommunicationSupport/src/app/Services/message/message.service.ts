import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from './../../../environments/environment';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Message } from 'src/app/Dtos/Message';
import { tokenKey } from 'src/app/constants/localStorageConsts';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  private rootRoute: string = environment.serverRoute + "message/"

  constructor(private client: HttpClient) { }

  messagesForUser(receiver: string): Observable<Message[]> {
    return this.client.get<Message[]>(`${this.rootRoute}messagesForUser/${receiver}`, { headers: this.getDefaultHeaders()});
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
