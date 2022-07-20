import { TestBed } from '@angular/core/testing';

import { Messaging.HubService } from './messaging.hub.service';

describe('Messaging.HubService', () => {
  let service: Messaging.HubService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Messaging.HubService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
