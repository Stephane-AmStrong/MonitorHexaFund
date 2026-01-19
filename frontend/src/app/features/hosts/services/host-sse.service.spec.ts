import { TestBed } from '@angular/core/testing';

import { HostSseService } from './host-sse.service';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { BaseApiService } from '../../../core/services/rest-api/base-api.service';

describe('HostSseService', () => {
  let service: HostSseService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        HostSseService,
        BaseApiService,
        provideHttpClient(),
        provideHttpClientTesting(),
        provideZonelessChangeDetection(),
      ],
    });
    service = TestBed.inject(HostSseService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
