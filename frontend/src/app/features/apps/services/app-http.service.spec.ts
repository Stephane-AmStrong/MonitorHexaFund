import { TestBed } from '@angular/core/testing';

import { AppHttpService } from './app-http.service';
import { provideZonelessChangeDetection } from '@angular/core';
import { BaseApiService } from '../../../core/services/rest-api/base-api.service';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';

describe('AppHttpService', () => {
  let service: AppHttpService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        AppHttpService,
        BaseApiService,
        provideHttpClient(),
        provideHttpClientTesting(),
        provideZonelessChangeDetection(),
      ],
    });
    service = TestBed.inject(AppHttpService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
