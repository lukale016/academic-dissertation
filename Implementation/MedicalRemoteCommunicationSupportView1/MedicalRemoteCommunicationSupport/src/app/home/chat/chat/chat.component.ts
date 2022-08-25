import { MatSnackBar } from '@angular/material/snack-bar';
import { HttpResponse, HttpErrorResponse } from '@angular/common/http';
import { FileService } from './../../../Services/file/file.service';
import { Subject, takeUntil, timeout, first } from 'rxjs';
import { MessageService } from './../../../Services/message/message.service';
import { MessagingHubService } from './../../../hubs/messaging/messaging.hub.service';
import { SuperUser } from './../../../models/SuperUser';
import { Component, Input, OnInit, OnDestroy, ViewChild, ElementRef, Output, EventEmitter } from '@angular/core';
import { Message } from 'src/app/Dtos/Message';
import { FileGuid } from 'src/app/Dtos/FileGuid';
import { DateTime } from 'luxon';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css']
})
export class ChatComponent implements OnInit, OnDestroy {
  public messages: { message: string, fromCurrent: boolean, timestamp: DateTime, fileHref: string }[] = []
  public newMessages: boolean = false;
  public messageContent!: string;
  private fileMessagePrefix: string = "file:";
  @Input() user!: SuperUser;
  @Input() chatUsername!: string;
  @Input() chatTitle!: string;
  @Output() notificationEvent = new EventEmitter(); 

  private canShowBadge = true;

  @ViewChild("chat") chat?: ElementRef;
  

  private $destroy: Subject<void> = new Subject<void>()

  constructor(private messagingHub: MessagingHubService, 
    private messageService: MessageService,
    private fileService: FileService,
    private snack: MatSnackBar) { }

  ngOnInit(): void {
    this.messageService.messagesForUser(this.chatUsername)
      .pipe(takeUntil(this.$destroy))
        .subscribe((messages: Message[]) => {
          this.messages = messages.map(m => this.mapMessage(m));
          if(this.chat)
          {
            this.chat.nativeElement.scrollTop = this.chat.nativeElement.scrollHeight;
          }
          this.messagingHub.$messages.pipe(takeUntil(this.$destroy))
            .subscribe((message: Message) => {
              if(message.from != this.user.username && message.from != this.chatUsername)
                return;
              let mappedMessage = this.mapMessage(message);
              this.messages.push(mappedMessage);
              if(!mappedMessage.fromCurrent && this.canShowBadge)
              {
                this.newMessages = true;
                this.notificationEvent.emit();
              }
              setTimeout(() => {
                if(this.chat)
                  this.chat.nativeElement.scrollTop = this.chat.nativeElement.scrollHeight
              }, 200);
            });
        });
  }

  sendMessage() {
    if(!this.messageContent || this.messageContent === "")
      return;
    this.messagingHub.sendMessage(this.chatUsername, this.messageContent);
    this.messageContent = "";
  }

  private mapMessage(message: Message): { message: string, fromCurrent: boolean, timestamp: DateTime, fileHref: string }  {
    let content: string = message.content;
    let href: string = "";
    if(content.startsWith(this.fileMessagePrefix))
    {
      let values: string[] = message.content.substring(this.fileMessagePrefix.length).split(":")
      content = values[0];
      values.shift();
      href = values.join(":");
    }
    return { 
      message: content, 
      fromCurrent: message.from === this.user.username ? true : false,
      timestamp: DateTime.fromISO(message.timeSent),
      fileHref: href
    }
  }

  chatOpened() {
    this.canShowBadge = false;
    this.newMessages = false
    if(this.chat)
      this.chat.nativeElement.scrollTop = this.chat.nativeElement.scrollHeight;
  }

  chatClosed() {
    this.canShowBadge = true;
  }
  
  onFileSelected(event: Event) {
    let source: HTMLInputElement = event.currentTarget as HTMLInputElement;
    if(!source || !source.files)
      return;
    let file: File = source.files[0];
    let data: FormData = new FormData();
    data.append("file", file, file.name);
    this.fileService.uploadFile(data).pipe(takeUntil(this.$destroy)).subscribe({
      next: (fileGuid: FileGuid) => {
        if(fileGuid.guid)
          this.messagingHub.sendMessage(this.chatUsername, `${this.fileMessagePrefix}${file.name}:${this.fileService.getFileUrl(fileGuid.guid)}`);
      },
      error: (error: HttpErrorResponse) => this.snack.open(error.message, "close", { duration: 2000 })
    });
    source.value = source.defaultValue;
  }

  formatTimeSent(timestamp: DateTime): string {

    return `${timestamp.hour}:${timestamp.minute}`;
  }

  ngOnDestroy(): void {
    this.$destroy.next();
    this.$destroy.complete();
  }
}
