import { TestBed } from '@angular/core/testing';

import { AppSseService } from './app-sse.service';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { BaseApiService } from '../../../core/services/rest-api/base-api.service';

describe('AppSseService', () => {
  let service: AppSseService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        AppSseService,
        BaseApiService,
        provideHttpClient(),
        provideHttpClientTesting(),
        provideZonelessChangeDetection(),
      ],
    });
    service = TestBed.inject(AppSseService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
