import { UserService } from './../../Services/user/user.service';
import { UserContainerService } from './../../container/userContainer/user-container.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { HttpErrorResponse } from '@angular/common/http';
import { CommentService } from './../../Services/comment/comment.service';
import { ModelValidator } from './../../helpers/ModelValidator';
import { SuperUser } from './../../models/SuperUser';
import { Comment } from './../../models/Comment';
import { Topic } from './../../models/Topic';
import { TopicService } from './../../Services/topic/topic.service';
import { environment } from './../../../environments/environment';
import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { mergeMap, Subject, takeUntil, EMPTY, map } from 'rxjs';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';

@Component({
  selector: 'app-topic',
  templateUrl: './topic.component.html',
  styleUrls: ['./topic.component.css']
})
export class TopicComponent implements OnInit, OnDestroy {
  public appName: string = environment.appName;
  public topic?: Topic;
  public comment: Comment = new Comment();
  public user?: SuperUser

  private $destroy: Subject<void> = new Subject<void>();

  constructor(private topicService: TopicService,
    private userService: UserService,
    private userContainer: UserContainerService,
    private commentService: CommentService,
    private snack: MatSnackBar,
    private route: ActivatedRoute,
    private router: Router,
    private changeDetector: ChangeDetectorRef) { }

  ngOnInit(): void {
    this.route.paramMap.pipe(
      takeUntil(this.$destroy),
      map((params: ParamMap) => params.get("id") ? params.get("id")! : ""),
      mergeMap((id: string) => {
        if(id === "")
          return EMPTY;
        return this.topicService.getTopic(Number.parseInt(id))
      })
      ).subscribe({
        next: (topic: Topic) => {
          this.topic = topic;
        },
        error: (error: HttpErrorResponse) => {
          this.snack.open(error.error, "close", { duration: 2000 });
          this.router.navigate([""]);
        }
      })

      this.userContainer.$user.pipe(takeUntil(this.$destroy))
        .subscribe((user: SuperUser | undefined) => {
          if(!user)
            this.userService.loadUserByToken();
          this.user = user
          this.changeDetector.detectChanges();
        });
  }

  addComment() {
    this.comment.owner = this.user!.username;
    this.comment.isDoctorComment = this.user!.isDoctor;
    this.comment.userFullName = this.user!.fullName;
    if(ModelValidator.validate(this.comment))
    {
      this.commentService.addComment(this.comment, this.topic!.id)
        .pipe(takeUntil(this.$destroy))
        .subscribe({
           next:  (comment: Comment) => this.topic!.comments.unshift(comment),
           error: (error: HttpErrorResponse) => this.snack.open(error.error, "close", { duration: 2000 })
        });
    }
  }

  ngOnDestroy(): void {
    this.$destroy.next();
    this.$destroy.complete();
  }
}
