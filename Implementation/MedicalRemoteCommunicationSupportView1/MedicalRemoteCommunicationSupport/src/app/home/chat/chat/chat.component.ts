import { Subject, takeUntil, tap } from 'rxjs';
import { MessageService } from './../../../Services/message/message.service';
import { MessagingHubService } from './../../../hubs/messaging/messaging.hub.service';
import { SuperUser } from './../../../models/SuperUser';
import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { Message } from 'src/app/Dtos/Message';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css']
})
export class ChatComponent implements OnInit, OnDestroy {
  public messages: { message: string, fromCurrent: boolean, timestamp: Date }[] = []
  public newMessages: boolean = false;
  public messageContent!: string;
  @Input() user!: SuperUser;
  @Input() chatUsername!: string;
  @Input() chatTitle!: string;
  

  private $destroy: Subject<void> = new Subject<void>()

  constructor(private messagingHub: MessagingHubService, private messageService: MessageService) { }

  ngOnInit(): void {
    this.messageService.messagesForUser(this.chatUsername)
      .pipe(takeUntil(this.$destroy),
        tap<Message[]>((messages: Message[]) => this.messages = messages.map(m => this.mapMessage(m))),
        tap(() => {
          this.messagingHub.$messages.pipe(takeUntil(this.$destroy))
            .subscribe((message: Message) => {
              this.messages.push(this.mapMessage(message));
              this.newMessages = true;
            });
        }));
  }

  sendMessage() {
    if(!this.messageContent || this.messageContent === "")
      return;
    this.messagingHub.sendMessage(this.chatUsername, this.messageContent);
  }

  private mapMessage(message: Message): { message: string, fromCurrent: boolean, timestamp: Date }  {
    return { 
      message: message.content, 
      fromCurrent: message.from === this.user.username ? true : false,
      timestamp: message.timeSent
    }
  }

  ngOnDestroy(): void {
    this.$destroy.next();
    this.$destroy.complete();
  }
}
