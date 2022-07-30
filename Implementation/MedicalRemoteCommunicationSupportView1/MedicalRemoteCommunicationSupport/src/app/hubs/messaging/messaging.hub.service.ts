import { tokenKey } from './../../constants/localStorageConsts';
import { Subject, Observable } from 'rxjs';
import { environment } from './../../../environments/environment';
import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder, IHttpConnectionOptions, IHubProtocol, LogLevel } from '@microsoft/signalr';
import { Message } from 'src/app/Dtos/Message';
import * as signalR from '@microsoft/signalr';

@Injectable({
  providedIn: 'root'
})
export class MessagingHubService {
  private connection?: HubConnection;
  private connectionRoute = environment.hubRoutes + "messaging";
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
      },
      transport: signalR.HttpTransportType.ServerSentEvents | signalR.HttpTransportType.LongPolling
    }
    if(options.accessTokenFactory!() === "")
      return;
    this.connection = new HubConnectionBuilder().withUrl(this.connectionRoute, options)
                                                //.configureLogging(LogLevel.Debug)
                                                .withAutomaticReconnect()
                                                .build();
    this.registerListeners();
    this.connection.start()
  }

  private registerListeners() {
    this.connection?.on("receiveMessage", (from: string, content: string, timeSent: Date) => {
      if(!from)
        return;
      this.messages.next(new Message(from, content, timeSent));
    });
  }

  Disconnect() {
    this.connection?.stop();
  }

  sendMessage(receiver: string, content: string) {
    if(!this.connection)
      this.Connect();
    this.connection?.send("sendMessage", receiver, content);
  }

  sendRequest(doctor: string) {
    if(!this.connection)
      this.Connect();
    this.connection?.send("sendRequest", doctor);
  }  

  acceptRequest(patient: string){
    if(!this.connection)
      this.Connect();
    this.connection?.send("acceptRequest", patient);
  }
}
