import { TestBed } from '@angular/core/testing';

import { AppStatusHttpService } from './app-status-http.service';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { BaseApiService } from '../../../core/services/rest-api/base-api.service';

describe('AppStatusHttpService', () => {
  let service: AppStatusHttpService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        AppStatusHttpService,
        BaseApiService,
        provideHttpClient(),
        provideHttpClientTesting(),
        provideZonelessChangeDetection(),
      ],
    });
    service = TestBed.inject(AppStatusHttpService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
