import { tokenKey } from './../../constants/localStorageConsts';
import { Subject, Observable } from 'rxjs';
import { environment } from './../../../environments/environment';
import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder, IHttpConnectionOptions } from '@microsoft/signalr';
import { Message } from 'src/app/Dtos/Message';

@Injectable({
  providedIn: 'root'
})
export class MessagingHubService {
  private connection?: HubConnection;
  private connectionRoute = environment.hubRoutes + "mesagging";
  private messages: Subject<Message> = new Subject<Message>();
  public $messages: Observable<Message> = this.messages.asObservable();

  constructor() { }

  Connect() {
    const options: IHttpConnectionOptions = {
      accessTokenFactory: () => {
        let token = localStorage.getItem(tokenKey);
        if(!token)
          return "";
        return token;
      }
    }
    if(options.accessTokenFactory!() === "")
      return;
    this.connection = new HubConnectionBuilder().withUrl(this.connectionRoute, options).build();
    this.registerListeners();
    this.connection.start()
  }

  Disconnect() {
    this.connection?.stop();
  }

  sendMessage(receiver: string, content: string) {
    if(!this.connection)
      this.Connect();
    this.connection?.send("sendMessage", receiver, content);
  }

  private registerListeners() {
    this.connection?.on("receiveMessage", (from: string, content: string, timeSent: Date) => {
      if(!from)
        return;
      this.messages.next(new Message(from, content, timeSent));
    });
  }
}
