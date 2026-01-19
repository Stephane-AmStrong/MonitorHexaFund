import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AppStatusTableComponent } from './app-status-table.component';
import { provideZonelessChangeDetection } from '@angular/core';

describe('AppStatusTableComponent', () => {
  let component: AppStatusTableComponent;
  let fixture: ComponentFixture<AppStatusTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AppStatusTableComponent],
      providers: [
        provideZonelessChangeDetection(),
      ],
    })
    .compileComponents();

    fixture = TestBed.createComponent(AppStatusTableComponent);
    component = fixture.componentInstance;
    fixture.componentRef.setInput('appStatuses', []);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
