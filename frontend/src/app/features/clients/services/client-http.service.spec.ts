import { TestBed } from '@angular/core/testing';

import { ClientHttpService } from './client-http.service';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { BaseApiService } from '../../../core/services/rest-api/base-api.service';

describe('ClientHttpService', () => {
  let service: ClientHttpService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        ClientHttpService,
        BaseApiService,
        provideHttpClient(),
        provideHttpClientTesting(),
        provideZonelessChangeDetection(),
      ],
    });
    service = TestBed.inject(ClientHttpService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
