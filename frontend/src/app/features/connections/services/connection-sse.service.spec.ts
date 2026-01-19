import { TestBed } from '@angular/core/testing';

import { ConnectionSseService } from './connection-sse.service';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { BaseApiService } from '../../../core/services/rest-api/base-api.service';

describe('ConnectionSseService', () => {
  let service: ConnectionSseService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        ConnectionSseService,
        BaseApiService,
        provideHttpClient(),
        provideHttpClientTesting(),
        provideZonelessChangeDetection(),
      ],
    });
    service = TestBed.inject(ConnectionSseService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
