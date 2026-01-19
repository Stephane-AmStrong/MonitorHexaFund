import { TestBed } from '@angular/core/testing';

import { AlertSseService } from './alert-sse.service';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { BaseApiService } from '../../../core/services/rest-api/base-api.service';

describe('AlertSseService', () => {
  let service: AlertSseService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        AlertSseService,
        BaseApiService,
        provideHttpClient(),
        provideHttpClientTesting(),
        provideZonelessChangeDetection(),
      ],
    });
    service = TestBed.inject(AlertSseService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
