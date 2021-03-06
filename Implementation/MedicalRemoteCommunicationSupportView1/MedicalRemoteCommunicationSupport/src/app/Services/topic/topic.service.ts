import { tokenKey } from './../../constants/localStorageConsts';
import { Topic } from './../../models/Topic';
import { Observable } from 'rxjs';
import { environment } from './../../../environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class TopicService {
  private rootRoute: string = environment.serverRoute + "topic/";

  constructor(private client: HttpClient) { }

  getTopics(): Observable<Topic[]> {
    return this.client.get<Topic[]>(`${this.rootRoute}GetTopics`, { headers: this.getDefaultHeaders() });
  }

  getTopic(id: number) {
    return this.client.get<Topic>(`${this.rootRoute}GetTopic/${id}`, { headers: this.getDefaultHeaders() });
  }

  addTopic(topic: Topic) {
    return this.client.post<Topic>(`${this.rootRoute}AddTopic`, topic, { headers: this.getDefaultHeaders() });
  }

  deleteTopic(id: number) {
    return this.client.delete<number>(`${this.rootRoute}DeleteTopic/${id}`, { headers: this.getDefaultHeaders() });
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
