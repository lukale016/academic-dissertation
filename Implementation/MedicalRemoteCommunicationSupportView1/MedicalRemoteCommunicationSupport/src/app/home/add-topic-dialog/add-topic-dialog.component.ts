import { Topic } from './../../models/Topic';
import { HomeComponent } from './../home/home.component';
import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-add-topic-dialog',
  templateUrl: './add-topic-dialog.component.html',
  styleUrls: ['./add-topic-dialog.component.css']
})
export class AddTopicDialogComponent implements OnInit {

  constructor(
    private dialogRef: MatDialogRef<HomeComponent>, 
    @Inject(MAT_DIALOG_DATA) public data: Topic
    ) { }

  ngOnInit(): void {
  }

  onNoClick() {
    this.dialogRef.close();
  }
}
