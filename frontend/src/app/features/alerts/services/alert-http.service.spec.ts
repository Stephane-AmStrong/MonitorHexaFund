import { TestBed } from '@angular/core/testing';

import { AlertHttpService } from './alert-http.service';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { BaseApiService } from '../../../core/services/rest-api/base-api.service';

describe('AlertHttpService', () => {
  let service: AlertHttpService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        AlertHttpService,
        BaseApiService,
        provideHttpClient(),
        provideHttpClientTesting(),
        provideZonelessChangeDetection(),
      ],
    });
    service = TestBed.inject(AlertHttpService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
