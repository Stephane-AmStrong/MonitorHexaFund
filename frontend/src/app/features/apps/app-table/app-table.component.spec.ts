import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AppTableComponent } from './app-table.component';
import { provideZonelessChangeDetection } from '@angular/core';

describe('AppTableComponent', () => {
  let component: AppTableComponent;
  let fixture: ComponentFixture<AppTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AppTableComponent],
      providers: [
        provideZonelessChangeDetection(),
      ],
    })
    .compileComponents();

    fixture = TestBed.createComponent(AppTableComponent);
    component = fixture.componentInstance;
    fixture.componentRef.setInput('apps', []);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
