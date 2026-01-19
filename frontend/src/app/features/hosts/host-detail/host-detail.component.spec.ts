import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HostDetailComponent } from './host-detail.component';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { BaseApiService } from '../../../core/services/rest-api/base-api.service';
import { HostHttpService } from '../services/host-http.service';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';

describe('HostDetailComponent', () => {
  let component: HostDetailComponent;
  let fixture: ComponentFixture<HostDetailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HostDetailComponent],
      providers: [
        HostHttpService,
        BaseApiService,
        provideHttpClient(),
        provideHttpClientTesting(),
        provideZonelessChangeDetection(),
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              params: {},
              queryParams: {},
            },
            params: of({}),
            queryParams: of({}),
          },
        },
      ],
    })
    .compileComponents();

    fixture = TestBed.createComponent(HostDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
