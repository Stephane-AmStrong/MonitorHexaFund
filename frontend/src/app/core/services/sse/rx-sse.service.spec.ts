import { TestBed } from '@angular/core/testing';
import { RxSseService } from './rx-sse.service';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { BaseApiService } from '../rest-api/base-api.service';


describe('RxSseService', () => {
  let service: RxSseService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        RxSseService,
        BaseApiService,
        provideHttpClient(),
        provideHttpClientTesting(),
        provideZonelessChangeDetection(),
      ],
    });
    service = TestBed.inject(RxSseService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
