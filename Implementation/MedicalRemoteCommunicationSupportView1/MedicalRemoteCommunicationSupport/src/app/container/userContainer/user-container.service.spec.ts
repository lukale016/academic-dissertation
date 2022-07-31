import { TestBed } from '@angular/core/testing';

import { UserContainerService } from './user-container.service';

describe('UserContainerService', () => {
  let service: UserContainerService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(UserContainerService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
