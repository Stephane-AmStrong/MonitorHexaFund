import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HostTableComponent } from './host-table.component';
import { provideZonelessChangeDetection } from '@angular/core';

describe('HostTableComponent', () => {
  let component: HostTableComponent;
  let fixture: ComponentFixture<HostTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HostTableComponent],
      providers: [
        provideZonelessChangeDetection(),
      ],
    })
    .compileComponents();

    fixture = TestBed.createComponent(HostTableComponent);
    component = fixture.componentInstance;
    fixture.componentRef.setInput('hosts', []);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
