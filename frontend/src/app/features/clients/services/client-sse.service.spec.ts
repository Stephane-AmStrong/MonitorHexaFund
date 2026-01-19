import { TestBed } from '@angular/core/testing';

import { ClientSseService } from './client-sse.service';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { BaseApiService } from '../../../core/services/rest-api/base-api.service';

describe('ClientSseService', () => {
  let service: ClientSseService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        ClientSseService,
        BaseApiService,
        provideHttpClient(),
        provideHttpClientTesting(),
        provideZonelessChangeDetection(),
      ],
    });
    service = TestBed.inject(ClientSseService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
