import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddTopicDialogComponent } from './add-topic-dialog.component';

describe('AddTopicDialogComponent', () => {
  let component: AddTopicDialogComponent;
  let fixture: ComponentFixture<AddTopicDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AddTopicDialogComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddTopicDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
