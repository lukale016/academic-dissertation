import { UserContainerService } from './../../container/userContainer/user-container.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { SuperUser } from './../../models/SuperUser';
import { tokenKey } from './../../constants/localStorageConsts';
import { Subject, Observable, takeUntil } from 'rxjs';
import { environment } from './../../../environments/environment';
import { Injectable, OnDestroy } from '@angular/core';
import { HubConnection, HubConnectionBuilder, IHttpConnectionOptions, IHubProtocol, LogLevel } from '@microsoft/signalr';
import { Message } from 'src/app/Dtos/Message';
import * as signalR from '@microsoft/signalr';

@Injectable({
  providedIn: 'root'
})
export class MessagingHubService implements OnDestroy {
  private connection?: HubConnection;
  private connectionRoute = environment.hubRoutes + "messaging";
  private user?: SuperUser;
  private $destroy: Subject<void> = new Subject<void>();
  private messages: Subject<Message> = new Subject<Message>();
  public $messages: Observable<Message> = this.messages.asObservable();

  constructor(private userContainer: UserContainerService,
    private snack: MatSnackBar) {
    userContainer.$user.pipe(takeUntil(this.$destroy))
      .subscribe((user: SuperUser | undefined) => this.user = user);
   }
  

  async Connect() {
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
    await this.connection.start()
  }

  private registerListeners() {
    this.connection?.on("receiveMessage", (from: string, content: string, timeSent: string) => {
      if(!from)
        return;
      this.messages.next(new Message(from, content, timeSent));
    });

    this.connection?.on("requestReceived", (username: string, fullName: string) => {
      if(!this.user)
        return;
      this.user.requests.push({ username, fullName });
      this.userContainer.user.next(this.user);
      this.snack.open(`${fullName} wants to be your patient.`, "ok", { duration: 2000 });
    });

    this.connection?.on("requestSent", (doctor: string, specialization: string) => {
      if(!this.user)
        return;
      this.user.sentRequests.push({ username: doctor, specialization });
      this.userContainer.user.next(this.user);
    });

    this.connection?.on("requestFinished", (doctor: string, fullName: string, skypeId: string) => {
      if(!this.user)
        return;
      this.user.sentRequests = this.user.sentRequests.filter(req => req.username != doctor);
      this.user.patients.push({ username: doctor, fullName, skypeId });
      this.userContainer.user.next(this.user);
      this.snack.open(`${fullName} accepted you as his patient.`, "ok", { duration: 2000 });
    });

    this.connection?.on("requestAccepted", (patient: string, fullName: string, skypeId: string) => {
      if(!this.user)
        return;
      this.user.requests = this.user.requests.filter(req => req.username != patient);
      this.user.myDoctors.push({ username: patient, fullName, skypeId });
      this.userContainer.user.next(this.user);
    });

    this.connection?.on("requestHasBeenRejected", (doctor: string, fullName: string) => {
      if(!this.user)
        return;
      this.user.sentRequests = this.user.sentRequests.filter(req => req.username != doctor);
      this.userContainer.user.next(this.user);
      this.snack.open(`${fullName} has rejected your request.`, "ok", { duration: 2000 });
    });

    this.connection?.on("requestRejected", (patient: string) => {
      if(!this.user)
        return;
      this.user.requests = this.user.requests.filter(req => req.username != patient);
      this.userContainer.user.next(this.user);
    });
  }

  async Disconnect() {
    await this.connection?.stop();
  }

  async sendMessage(receiver: string, content: string) {
    if(this.connection?.state === signalR.HubConnectionState.Disconnected)
      await this.connection.start();
    this.connection?.send("sendMessage", receiver, content);
  }

  async sendRequest(doctor: string) {
    if(this.connection?.state === signalR.HubConnectionState.Disconnected)
      await this.connection.start();
    this.connection?.send("sendRequest", doctor);
  }  

  async acceptRequest(patient: string){
    if(this.connection?.state === signalR.HubConnectionState.Disconnected)
      await this.connection.start();
    this.connection?.send("acceptRequest", patient);
  }

  async rejectRequest(patient: string) {
    if(this.connection?.state === signalR.HubConnectionState.Disconnected)
      await this.connection.start();
    this.connection?.send("rejectRequest", patient);
  }
  
  ngOnDestroy(): void {
    this.$destroy.next();
    this.$destroy.complete();
    this.Disconnect();
  }
}
