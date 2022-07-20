import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css']
})
export class ChatComponent implements OnInit {

  public messages: {message: string, yours: boolean}[] = [{message: "Hello", yours: false}, {message: "Hello", yours: true}]

  constructor() { }

  // Open connection on user login or token login, split items messages by from attribute
  ngOnInit(): void {
  }

}
