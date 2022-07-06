import { Observable } from 'rxjs';
import { Comment } from './../../models/Comment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from './../../../environments/environment';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class CommentService {
  private rootRoute: string = environment.serverRoute + "comment/";

  constructor(private client: HttpClient) { }

  addComment(comment: Comment, postId: number): Observable<Comment> {
    return this.client.post<Comment>(`${this.rootRoute}addCommentToPost/${postId}`, comment, {headers: this.getDefaultHeaders()});
  }

  deleteComment(comment: Comment, postId: number) {
    return this.client.delete(`${this.rootRoute}deleteCommentToPost/${postId}`, {headers: this.getDefaultHeaders(), responseType: "text"});
  }

  private getDefaultHeaders() : HttpHeaders {
    let headers = new HttpHeaders();
    headers.append("ContentType", "application/json");
    headers.append("Allow", "*");
    return headers;
  }
}
