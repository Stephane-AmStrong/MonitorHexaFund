import { TestBed } from '@angular/core/testing';

import { ConnectionHttpService } from './connection-http.service';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { BaseApiService } from '../../../core/services/rest-api/base-api.service';

describe('ConnectionHttpService', () => {
  let service: ConnectionHttpService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        ConnectionHttpService,
        BaseApiService,
        provideHttpClient(),
        provideHttpClientTesting(),
        provideZonelessChangeDetection(),
      ],
    });
    service = TestBed.inject(ConnectionHttpService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
