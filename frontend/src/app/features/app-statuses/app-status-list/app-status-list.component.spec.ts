import { ComponentFixture, TestBed } from '@angular/core/testing';

import { provideZonelessChangeDetection } from '@angular/core';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { BaseApiService } from '../../../core/services/rest-api/base-api.service';
import { AppStatusListComponent } from './app-status-list.component';
import {AppStatusHttpService} from '../services/app-status-http.service';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';

describe('AppStatusListComponent', () => {
  let component: AppStatusListComponent;
  let fixture: ComponentFixture<AppStatusListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AppStatusListComponent],
      providers: [
        AppStatusHttpService,
        BaseApiService,
        provideHttpClient(),
        provideHttpClientTesting(),
        provideZonelessChangeDetection(),
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              params: {
                withName: 'test-host',
                withAppName: 'test-app',
                withVersion: '1.0.0',
              },
              queryParams: {
                searchTerm: '',
                orderBy: 'name',
                page: 1,
                pageSize: 10,
              },
            },
            queryParams: of({
              searchTerm: '',
              orderBy: 'name',
              page: 1,
              pageSize: 10,
            }),
          },
        },
      ],
    })
    .compileComponents();

    fixture = TestBed.createComponent(AppStatusListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
