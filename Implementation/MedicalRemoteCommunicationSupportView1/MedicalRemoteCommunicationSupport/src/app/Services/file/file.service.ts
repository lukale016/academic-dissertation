import { FileGuid } from './../../Dtos/FileGuid';
import { Observable } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from './../../../environments/environment';
import { Injectable } from '@angular/core';
import { tokenKey } from 'src/app/constants/localStorageConsts';

@Injectable({
  providedIn: 'root'
})
export class FileService {
  private rootRoute: string = environment.serverRoute + "file/";

  constructor(private client: HttpClient) { }

  uploadFile(file: FormData): Observable<FileGuid> {
    return this.client.post<FileGuid>(`${this.rootRoute}storeFile`, file, {
      headers: this.getDefaultHeaders()
    })
  }

  getFileUrl(fileName: string) {
    return `${this.rootRoute}getFile/${fileName}`;
  }


  private getDefaultHeaders() : HttpHeaders {
    let headers = new HttpHeaders();
    headers = headers.append("ContentType", "multipart/form-data")
                     .append("Allow", "*");
    const token: string | null = localStorage.getItem(tokenKey);
    if(token)
      headers = headers.append("Authorization", `Bearer ${token}`)
    return headers;
  }
}
