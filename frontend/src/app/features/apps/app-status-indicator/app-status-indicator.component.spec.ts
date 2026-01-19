import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AppStatusIndicatorComponent } from './app-status-indicator.component';
import { provideZonelessChangeDetection } from '@angular/core';

describe('AppStatusIndicatorComponent', () => {
  let component: AppStatusIndicatorComponent;
  let fixture: ComponentFixture<AppStatusIndicatorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AppStatusIndicatorComponent],
      providers: [
        provideZonelessChangeDetection(),
      ],
    })
    .compileComponents();

    fixture = TestBed.createComponent(AppStatusIndicatorComponent);
    component = fixture.componentInstance;
    fixture.componentRef.setInput('status', 'Up');
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
