import { TestBed } from '@angular/core/testing';

import { AppStatusSseService } from './app-status-sse.service';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { BaseApiService } from '../../../core/services/rest-api/base-api.service';

describe('AppStatusSseService', () => {
  let service: AppStatusSseService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        AppStatusSseService,
        BaseApiService,
        provideHttpClient(),
        provideHttpClientTesting(),
        provideZonelessChangeDetection(),
      ],
    });
    service = TestBed.inject(AppStatusSseService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
