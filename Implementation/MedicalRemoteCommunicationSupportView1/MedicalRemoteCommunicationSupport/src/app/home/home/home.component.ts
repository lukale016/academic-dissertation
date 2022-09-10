import { MessagingHubService } from './../../hubs/messaging/messaging.hub.service';
import { UserContainerService } from './../../container/userContainer/user-container.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ModelValidator } from './../../helpers/ModelValidator';
import { AddTopicDialogComponent } from './../add-topic-dialog/add-topic-dialog.component';
import { environment } from './../../../environments/environment';
import { Topic } from './../../models/Topic';
import { TopicService } from './../../Services/topic/topic.service';
import { debounceTime, first, Subject, takeUntil } from 'rxjs';
import { SuperUser } from './../../models/SuperUser';
import { UserService } from './../../Services/user/user.service';
import { Component, OnInit, OnDestroy, ViewChild, AfterViewInit, ChangeDetectorRef } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatDialog } from '@angular/material/dialog';
import { MatDrawer } from '@angular/material/sidenav';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit, AfterViewInit, OnDestroy {
  public appName: string = environment.appName;
  public user?: SuperUser;
  public isDoctor: boolean = false; 
  private $destroy: Subject<void> = new Subject<void>();
  public topics: Topic[] = [];
  public topicSlice: Topic[] = [];
  public autocompleteOptions: string[] = this.topics.map((t: Topic) => t.title);
  public searchValue: FormControl = new FormControl("");
  public showBadge = false;
  @ViewChild("chatContainer") chatContainer?: MatDrawer;

  @ViewChild(MatPaginator) topicPages!: MatPaginator

  constructor(
    private userService: UserService,
    private userContainer: UserContainerService, 
    private topicService: TopicService, 
    private dialog: MatDialog,
    private snack: MatSnackBar,
    private changeDetector: ChangeDetectorRef,
    private messageHub: MessagingHubService
    ) { }

  ngOnInit(): void {
    this.userService.$isDoctor.pipe(takeUntil(this.$destroy))
                    .subscribe((isDoctor: boolean) => this.isDoctor = isDoctor);
    this.userContainer.$user.pipe(takeUntil(this.$destroy))
                    .subscribe((user: SuperUser | undefined) => {
                      if(!user)
                        this.userService.loadUserByToken();
                      this.user = user;
                      this.changeDetector.detectChanges();
                    }
                    );
    this.searchValue.valueChanges.pipe(takeUntil(this.$destroy))
                    .subscribe((searchValue: string) => this._filterAutocomplete(searchValue));
    this.searchValue.valueChanges.pipe(
                      debounceTime(500),
                      takeUntil(this.$destroy)
                      )
                      .subscribe((searchValue: string) => this._filterTopics(searchValue));
  }

  ngAfterViewInit(): void {
    this.topicService.getTopics().pipe(takeUntil(this.$destroy))
                     .subscribe((topics: Topic[]) => {
                        this.topics = topics
                        this.topicSlice = this.topics.slice(0, this.topicPages.pageSize);
                     });
    this.topicPages.page.pipe(takeUntil(this.$destroy))
                   .subscribe(() => {
                    let page: number = this.topicPages.pageIndex;
                    let pageSize: number = this.topicPages.pageSize;
                    this.topicSlice = this.topics.slice(page * pageSize, (page + 1) * pageSize);
                   })
  }

  private _filterAutocomplete(searchValue: string) {
    this.autocompleteOptions = this.topics.map((topic: Topic) => topic.title)
                                          .filter((title: string) => title.toLocaleLowerCase().includes(searchValue.toLocaleLowerCase()));
  } 

  private _filterTopics(searchValue: string) {
    this.topicSlice = this.topics.filter((topic: Topic) => topic.title.toLocaleLowerCase().includes(searchValue.toLocaleLowerCase()));
  }

  openAddTopicDialog() {
    let topic: Topic = new Topic();
    if(!this.user)
      return;
    topic.owner = this.user!.username;
    const dialogRef = this.dialog.open(AddTopicDialogComponent, {
      width: "800px",
      data: topic
    });

    dialogRef.afterClosed().pipe(first()).subscribe((topic: Topic) =>{
      if(!ModelValidator.validate(topic))
      {
        this.snack.open("Fill all fields", "close", { duration: 2000 }); 
        return;
      }
      this.topicService.addTopic(topic).pipe(takeUntil(this.$destroy))
                       .subscribe((newTopic: Topic) => {
                          this.snack.open("Added new topic", "close", { duration: 2000 }); 
                          this.topics.unshift(newTopic);
                          this.user?.createdTopics.unshift(newTopic);
                          this._filterTopics(this.searchValue.value);
                       });
    });
  }

  logout() {
    this.userService.logout();
  }

  async acceptRequest(patient: string) {
    await this.messageHub.acceptRequest(patient);
  }
  
  async rejectRequest(patient: string) {
    await this.messageHub.rejectRequest(patient);
  }

  messageArrived() {
    this.showBadge = true;
  }

  openChats() {
    this.chatContainer?.toggle();
    this.showBadge = false;
  }

  ngOnDestroy(): void {
    this.$destroy.next();
    this.$destroy.complete();
  }

}
