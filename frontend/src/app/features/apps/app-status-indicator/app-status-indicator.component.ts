import { Component, input } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';

@Component({
  selector: 'app-status-indicator',
  imports: [MatIconModule, MatListModule],
  templateUrl: './app-status-indicator.component.html',
  styleUrl: './app-status-indicator.component.scss',
})
export class AppStatusIndicatorComponent {
  status = input.required<'Up' | 'Down' | undefined>();
}
