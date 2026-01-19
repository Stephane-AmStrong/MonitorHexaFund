import { TestBed } from '@angular/core/testing';

import { HostHttpService } from './host-http.service';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { BaseApiService } from '../../../core/services/rest-api/base-api.service';

describe('HostHttpService', () => {
  let service: HostHttpService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        HostHttpService,
        BaseApiService,
        provideHttpClient(),
        provideHttpClientTesting(),
        provideZonelessChangeDetection(),
      ],
    });
    service = TestBed.inject(HostHttpService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
